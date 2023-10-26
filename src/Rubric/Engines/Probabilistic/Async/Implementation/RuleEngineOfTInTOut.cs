using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Engines.Probabilistic.Implementation;
using Rubric.Rules.Probabilistic.Async;
using Rubric.Rulesets.Probabilistic.Async;

namespace Rubric.Engines.Probabilistic.Async.Implementation;

/// <inheritdoc cref="IRuleEngine{TIn,TOut}" />
public class RuleEngine<TIn, TOut> : BaseProbabilisticRuleEngine, IRuleEngine<TIn, TOut>
  where TIn : class
  where TOut : class
{
#region Fields

  private readonly IRule<TOut>[][] _postRules;

  private readonly IRule<TIn>[][] _preRules;

  private readonly IRule<TIn, TOut>[][] _rules;

#endregion

#region Constructors

  /// <summary>
  ///   Construct a rule engine from a ruleset.
  /// </summary>
  /// <param name="ruleset">A collection of various rules</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger</param>
  public RuleEngine(IRuleset<TIn, TOut> ruleset,
                    bool isParallel = false,
                    IExceptionHandler exceptionHandler = null,
                    ILogger logger = null)
    : this(ruleset?.PreRules, ruleset?.Rules, ruleset?.PostRules, isParallel, exceptionHandler, logger) { }

  /// <summary>
  ///   Default public constructor.
  /// </summary>
  /// <param name="preprocessingRules">Collection of synchronous preprocessing rules.</param>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="postprocessingRules">Collection of synchronous postprocessing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger.</param>
  public RuleEngine(
    IEnumerable<IRule<TIn>> preprocessingRules,
    IEnumerable<IRule<TIn, TOut>> rules,
    IEnumerable<IRule<TOut>> postprocessingRules,
    bool isParallel = false,
    IExceptionHandler exceptionHandler = null,
    ILogger logger = null
  )
  {
    preprocessingRules ??= Enumerable.Empty<IRule<TIn>>();
    _preRules =
      preprocessingRules.ResolveDependencies()
                        .Select(e => e.ToArray())
                        .ToArray();
    postprocessingRules ??= Enumerable.Empty<IRule<TOut>>();
    _postRules
      = postprocessingRules.ResolveDependencies()
                           .Select(e => e.ToArray())
                           .ToArray();
    rules ??= Enumerable.Empty<IRule<TIn, TOut>>();
    _rules
      = rules.ResolveDependencies()
             .Select(e => e.ToArray())
             .ToArray();
    IsParallel = isParallel;
    ExceptionHandler = exceptionHandler ?? ExceptionHandlers.Rethrow;
    Logger = logger ?? NullLogger.Instance;
  }

#endregion

#region Properties

  /// <inheritdoc />
  public bool IsParallel { get; internal set; }

  /// <inheritdoc />
  public override bool IsAsync => true;

  /// <inheritdoc />
  public override Type InputType => typeof(TIn);

  /// <inheritdoc />
  public override Type OutputType => typeof(TOut);

  /// <inheritdoc />
  public IEnumerable<IRule<TIn>> PreRules => _preRules.SelectMany(r => r);

  /// <inheritdoc />
  public IEnumerable<IRule<TIn, TOut>> Rules => _rules.SelectMany(r => r);

  /// <inheritdoc />
  public IEnumerable<IRule<TOut>> PostRules => _postRules.SelectMany(r => r);

#endregion

#region Public Methods

  /// <inheritdoc />
  public async Task ApplyAsync(
    TIn input,
    TOut output,
    IEngineContext context = null,
    CancellationToken token = default)
  {
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        await ApplyItemAsync(input, output, context, token).ConfigureAwait(false);
        using (Logger.BeginScope("Output", output))
        {
          await ApplyPostAsync(output, context, token).ConfigureAwait(false);
        }
      }
      catch (EngineException) { }
    }
  }

  /// <inheritdoc />
  public async Task ApplyAsync(
    IEnumerable<TIn> inputs,
    TOut output,
    IEngineContext context = null,
    CancellationToken token = default)
  {
    if (inputs == null) throw new ArgumentNullException(nameof(inputs));
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        await ApplyManyAsyncSerial(inputs, output, context, token).ConfigureAwait(false);
        using (Logger.BeginScope("Output", output))
        {
          await ApplyPostAsync(output, context, token).ConfigureAwait(false);
        }
      }
      catch (EngineException) { }
    }
  }

  /// <inheritdoc />
  public async Task ApplyParallelAsync(
    IEnumerable<TIn> inputs,
    TOut output,
    IEngineContext context,
    CancellationToken token = default)
  {
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        await ApplyManyAsyncParallel(inputs, output, context, token).ConfigureAwait(false);
      }
      catch (EngineException) { }
    }
  }

  /// <inheritdoc />
  public async Task ApplyAsync(
    IAsyncEnumerable<TIn> inputStream,
    TOut output,
    IEngineContext context,
    CancellationToken token = default)
  {
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        await foreach (var input in inputStream.WithCancellation(token))
          await ApplyItemAsync(input, output, context, token).ConfigureAwait(false);
        await ApplyPostAsync(output, context, token).ConfigureAwait(false);
      }
      catch (EngineException) { }
    }
  }

#endregion

#region Private Methods

  /// <summary>
  ///   Process a single item.
  /// </summary>
  /// <param name="i">The input.</param>
  /// <param name="o">The output.</param>
  /// <param name="ctx">The context.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task ApplyItemAsync(TIn i, TOut o, IEngineContext ctx, CancellationToken t)
  {
    Logger.BeginScope("Input", i);
    return IsParallel
      ? ApplyItemParallel(ctx, i, o, t)
      : ApplyItemSerial(ctx, i, o, t);
  }

  /// <summary>
  ///   Process a single item, running the async rules in sequence.
  /// </summary>
  /// <param name="ctx">The context.</param>
  /// <param name="i">The input.</param>
  /// <param name="o">The output.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private async Task ApplyItemSerial(IEngineContext ctx, TIn i, TOut o, CancellationToken t)
  {
    foreach (var set in _preRules)
      foreach (var rule in set)
      {
        t.ThrowIfCancellationRequested();
        await this.ApplyAsyncPreRule(ctx, rule, i, t).ConfigureAwait(false);
      }

    foreach (var set in _rules)
      foreach (var rule in set)
      {
        t.ThrowIfCancellationRequested();
        await this.ApplyAsyncRule(ctx, rule, i, o, t).ConfigureAwait(false);
      }
  }

  /// <summary>
  ///   Process a single item, running the async rules in parallel.
  /// </summary>
  /// <param name="ctx">The context.</param>
  /// <param name="i">The input.</param>
  /// <param name="o">The output.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private async Task ApplyItemParallel(IEngineContext ctx, TIn i, TOut o, CancellationToken t)
  {
    foreach (var set in _preRules)
    {
      t.ThrowIfCancellationRequested();
      await ParallelizePre(ctx, set, i, t).ConfigureAwait(false);
    }

    foreach (var set in _rules)
    {
      t.ThrowIfCancellationRequested();
      await Parallelize(ctx, set, i, o, t).ConfigureAwait(false);
    }
  }

  /// <summary>
  ///   Process the output.
  /// </summary>
  /// <param name="ctx">The context.</param>
  /// <param name="o">The output.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private async Task ApplyPostAsync(TOut o, IEngineContext ctx, CancellationToken t)
  {
    using (Logger.BeginScope("Output", o))
    {
      if (IsParallel)
        foreach (var set in _postRules)
        {
          t.ThrowIfCancellationRequested();
          await ParallelizePost(ctx, set, o, t).ConfigureAwait(false);
        }
      else
        foreach (var set in _postRules)
          foreach (var rule in set)
          {
            t.ThrowIfCancellationRequested();
            await this.ApplyAsyncPostRule(ctx, rule, o, t).ConfigureAwait(false);
          }
    }
  }

  /// <summary>
  ///   Apply several inputs serially.
  /// </summary>
  /// <param name="inputs">The inputs to process.</param>
  /// <param name="output">The output to process.</param>
  /// <param name="ctx">The execution context.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private async Task ApplyManyAsyncSerial(IEnumerable<TIn> inputs, TOut output, IEngineContext ctx, CancellationToken t)
  {
    foreach (var input in inputs)
    {
      t.ThrowIfCancellationRequested();
      using (Logger.BeginScope("Input", input))
      {
        try
        {
          await ApplyItemAsync(input, output, ctx, t).ConfigureAwait(false);
        }
        catch (ItemHaltException) { }
      }
    }
  }

  /// <summary>
  ///   Apply several inputs in parallel.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="inputs">The inputs to process.</param>
  /// <param name="o">The output to process.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private async Task ApplyManyAsyncParallel(IEnumerable<TIn> inputs, TOut o, IEngineContext ctx, CancellationToken t)
  {
    await ParallelizeInputs(ctx, inputs, o, t).ConfigureAwait(false);
    await ApplyPostAsync(o, ctx, t).ConfigureAwait(false);
  }

  /// <summary>
  ///   Parallelize a set of inputs.  Cancel other inputs when any other input throws.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="inputs">The input being processed.</param>
  /// <param name="o">The output object.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task ParallelizeInputs(IEngineContext ctx, IEnumerable<TIn> inputs, TOut o, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    var t2 = cts.Token;
    Exception userException = null;
    var tasks = inputs.Select(i => Task.Run(async () =>
    {
      t.ThrowIfCancellationRequested();
      try
      {
        await ApplyItemAsync(i, o, ctx, t2).ConfigureAwait(false);
      }
      catch (ItemHaltException) { }
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

  /// <summary>
  ///   Parallelize a set of preprocessing rules.  Cancel other rules when any rule throws.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="rules">The rules to parallelize.</param>
  /// <param name="i">The input being processed.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task ParallelizePre(IEngineContext ctx, IEnumerable<IRule<TIn>> rules, TIn i, CancellationToken t)
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

  /// <summary>
  ///   Parallelize a set of rules.  Cancel other rules when any rule throws.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="rules">The rules to parallelize.</param>
  /// <param name="i">The input being processed.</param>
  /// <param name="o">The output being processed.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task Parallelize(IEngineContext ctx, IEnumerable<IRule<TIn, TOut>> rules, TIn i, TOut o, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    var t2 = cts.Token;
    Exception userException = null;
    var tasks = rules.Select(
      r => Task.Run(async () =>
      {
        try
        {
          await this.ApplyAsyncRule(ctx, r, i, o, t2).ConfigureAwait(false);
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

  /// <summary>
  ///   Parallelize a set of postprocessing rules.  Cancel other rules when any rule throws.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="rules">The rules to parallelize.</param>
  /// <param name="o">The input being processed.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task ParallelizePost(IEngineContext ctx, IEnumerable<IRule<TOut>> rules, TOut o, CancellationToken t)
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
            await this.ApplyAsyncPostRule(ctx, r, o, t2).ConfigureAwait(false);
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

#endregion
}