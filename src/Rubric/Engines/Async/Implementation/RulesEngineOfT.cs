using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Engines.Implementation;
using Rubric.Rules.Async;
using Rubric.Rulesets.Async;

namespace Rubric.Engines.Async.Implementation;

/// <inheritdoc cref="IRuleEngine{T}" />
public class RuleEngine<T> : BaseRuleEngine, IRuleEngine<T>
  where T : class
{
#region Fields

  /// <summary>
  ///   Ordered and parallelized processing rules
  /// </summary>
  private readonly IRule<T>[][] _rules;

#endregion

#region Constructors

  /// <summary>
  ///   Convenience ruleset constructor.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="handler">An exception handler.</param>
  /// <param name="logger">A logger.</param>
  public RuleEngine(
    IRuleset<T> ruleSet,
    bool isParallel = false,
    IExceptionHandler handler = null,
    ILogger logger = null
  ) : this(
    null,
    ruleSet?.Rules,
    isParallel,
    handler,
    logger) { }

  /// <summary>
  ///   Full constructor.
  /// </summary>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">A logger.</param>
  public RuleEngine(
    IEnumerable<IRule<T>> asyncRules,
    bool isParallel = false,
    IExceptionHandler exceptionHandler = null,
    ILogger logger = null
  ) : this(null, asyncRules, isParallel, exceptionHandler, logger) { }

  /// <summary>
  ///   Full constructor.
  /// </summary>
  /// <param name="syncRules">Collection of synchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">A logger.</param>
  public RuleEngine(Rulesets.IRuleset<T> syncRules,
                    bool isParallel = false,
                    IExceptionHandler exceptionHandler = null,
                    ILogger logger = null
  ) : this(syncRules?.Rules, null, isParallel, exceptionHandler, logger) { }

  /// <summary>
  ///   Full constructor.
  /// </summary>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="handler">An optional exception handler.</param>
  /// <param name="logger">A logger.</param>
  private RuleEngine(
    IEnumerable<Rules.IRule<T>> rules,
    IEnumerable<IRule<T>> asyncRules,
    bool isParallel = false,
    IExceptionHandler handler = null,
    ILogger logger = null
  )
  {
    IsParallel = isParallel;
    _rules =
      (rules ?? Enumerable.Empty<Rules.IRule<T>>())
      .Select(r => r.WrapAsync())
      .Concat(asyncRules ??
              Enumerable.Empty<IRule<T>>())
      .ResolveDependencies()
      .Select(e => e.ToArray())
      .ToArray();
    Logger = logger ?? NullLogger.Instance;
    ExceptionHandler = handler ?? ExceptionHandlers.Rethrow;
  }

#endregion

#region Properties

  /// <inheritdoc />
  public bool IsParallel { get; }

  /// <inheritdoc />
  public override bool IsAsync => true;

  /// <inheritdoc />
  public override Type InputType => typeof(T);

  /// <inheritdoc />
  public override Type OutputType => typeof(T);

  /// <inheritdoc />
  public IEnumerable<IRule<T>> Rules => _rules.SelectMany(r => r);

#endregion

#region Public Methods

  /// <inheritdoc />
  public async Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default)
  {
    context = SetupContext(context);
    try
    {
      await ApplyItemAsync(input, context, token).ConfigureAwait(false);
    }
    catch (EngineException) { }
  }

  /// <inheritdoc />
  public async Task ApplyAsync(
    IEnumerable<T> inputs,
    IEngineContext context = null,
    bool parallelizeInputs = false,
    CancellationToken token = default)
  {
    if (inputs == null) throw new ArgumentNullException(nameof(inputs));
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        if (parallelizeInputs)
          await ApplyManyParallelAsync(context, inputs, token).ConfigureAwait(false);
        else
          await ApplyManySerialAsync(inputs, context, token).ConfigureAwait(false);
      }
      catch (EngineHaltException) { }
    }
  }

  /// <inheritdoc />
  public async Task ApplyAsync(
    IAsyncEnumerable<T> inputStream,
    IEngineContext context = null,
    CancellationToken token = default)
  {
    if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        await ApplyManyAsync(inputStream, context, token).ConfigureAwait(false);
      }
      catch (EngineHaltException) { }
    }
  }

#endregion

#region Private Methods

  private Task ApplyItemAsync(T input, IEngineContext context = null, CancellationToken token = default)
  {
    using (Logger.BeginScope("Input", input))
    {
      return IsParallel
        ? ApplyParallel(context, input, token)
        : ApplySerial(context, input, token);
    }
  }

  private async Task ApplySerial(IEngineContext ctx, T i, CancellationToken t)
  {
    foreach (var set in _rules)
      foreach (var rule in set)
      {
        t.ThrowIfCancellationRequested();
        try
        {
          await this.ApplyAsyncPreRule(ctx, rule, i, t).ConfigureAwait(false);
        }
        catch (ItemHaltException)
        {
          return;
        }
      }
  }

  private async Task ApplyParallel(IEngineContext ctx, T i, CancellationToken t)
  {
    foreach (var set in _rules)
    {
      t.ThrowIfCancellationRequested();
      await Parallelize(ctx, set, i, t).ConfigureAwait(false);
    }
  }

  private Task Parallelize(IEngineContext ctx, IEnumerable<IRule<T>> rules, T i, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    var t2 = cts.Token;
    Exception userException = null;
    var tasks =
      rules.Select(
        r => Task.Run(async () =>
        {
          try
          {
            await this.ApplyAsyncPreRule(ctx, r, i, t2).ConfigureAwait(false);
          }
          catch (Exception e)
          {
            userException = e;
            cts.Cancel();
            throw;
          }
        }, t2));
    return Task.WhenAll(tasks)
               .ContinueWith(task => ParallelCleanup(task, cts, userException),
                             t,
                             TaskContinuationOptions.HideScheduler,
                             TaskScheduler.Default);
  }

  private async Task ApplyManyAsync(IAsyncEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    await foreach (var input in inputs.WithCancellation(t))
      await ApplyItemAsync(input, context, t).ConfigureAwait(false);
  }

  private async Task ApplyManySerialAsync(IEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    foreach (var input in inputs)
    {
      t.ThrowIfCancellationRequested();
      await ApplyItemAsync(input, context, t).ConfigureAwait(false);
    }
  }

  private Task ApplyManyParallelAsync(IEngineContext ctx, IEnumerable<T> inputs, CancellationToken t)
    => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyItemAsync(i, ctx, t), t)));

#endregion
}