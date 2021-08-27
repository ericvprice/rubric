using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine;

public class AsyncRulesEngine<T> : IAsyncRulesEngine<T>
    where T : class
{
  /// <summary>
  ///     Ordered and parallelized processing rules
  /// </summary>
  private readonly IAsyncRule<T>[][] _rules;

  /// <summary>
  ///     Convenience ruleset constructor.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      AsyncRuleset<T> ruleSet,
      bool isParallel = false,
      IExceptionHandler handler = null,
      ILogger logger = null
  ) : this(
      null,
      ruleSet.AsyncRules,
      isParallel,
      handler,
      logger)
  { }

  /// <summary>
  ///     Convenience ruleset constructor.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      Ruleset<T> ruleSet,
      bool isParallel = false,
      IExceptionHandler handler = null,
      ILogger logger = null
  ) : this(
      ruleSet.Rules,
      null,
      isParallel,
      handler,
      logger)
  { }

  /// <summary>
  ///     Full constructor.
  /// </summary>
  /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      IEnumerable<IAsyncRule<T>> asyncRules,
      bool isParallel = false,
      IExceptionHandler handler = null,
      ILogger logger = null
  ) : this(null, asyncRules, isParallel, handler, logger) { }


  /// <summary>
  ///     Full constructor.
  /// </summary>
  /// <param name="preRules">Collection of synchronous preprocessing rules.</param>
  /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="postRules">Collection of synchronous postprocessing rules.</param>
  /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      IEnumerable<IRule<T>> rules,
      IEnumerable<IAsyncRule<T>> asyncRules,
      bool isParallel = false,
      IExceptionHandler handler = null,
      ILogger logger = null
  )
  {
    IsParallel = isParallel;
    _rules =
        (rules ?? Enumerable.Empty<IRule<T>>()).Select(r => r.WrapAsync())
                                                    .Concat(asyncRules ??
                                                            Enumerable.Empty<IAsyncRule<T>>())
                                                    .ResolveDependencies()
                                                    .Select(e => e.ToArray())
                                                    .ToArray();
    Logger = logger ?? NullLogger.Instance;
    ExceptionHandler = handler ?? ExceptionHandlers.Throw;
  }
  public bool IsParallel { get; internal set; }

  public bool IsAsync => true;

  /// <inheritdoc />
  public Type InputType => typeof(T);

  /// <inheritdoc />
  public Type OutputType => typeof(T);

  public IEnumerable<IAsyncRule<T>> Rules => _rules.SelectMany(_ => _);

  public ILogger Logger { get; }

  public IExceptionHandler ExceptionHandler { get; }

  public EngineException LastException { get; }

  public Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default)
  {
    var ctx = context ?? new EngineContext();
    SetupContext(ctx);
    return IsParallel
        ? ApplyParallel(input, ctx, token)
        : ApplySerial(input, ctx, token);
  }

  public Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null, bool parallelizeInputs = false, CancellationToken token = default)
  {
    var ctx = context ?? new EngineContext();
    SetupContext(ctx);
    if (IsParallel)
      if (parallelizeInputs)
        return ApplyParallelManyAsyncParallel(inputs, ctx, token);
      else
        return ApplyManyAsyncParallel(inputs, ctx, token);
    else
        if (parallelizeInputs)
      return ApplyParallelManyAsyncSerial(inputs, ctx, token);
    else
      return ApplyManyAsyncSerial(inputs, ctx, token);
  }

  private async Task ApplyRule(IEngineContext context, IAsyncRule<T> rule, T input, CancellationToken token)
  {
    try
    {
      var doesApply = await rule.DoesApply(context, input, token).ConfigureAwait(false);
      if (doesApply)
      {
        try
        {
          using (var logCtx = Logger.BeginScope(rule.Name))
          {
            Logger.LogTrace($"Rule {rule.Name} applies.");
            Logger.LogTrace($"Applying {rule.Name}.");
            await rule.Apply(context, input, token).ConfigureAwait(false);
            Logger.LogTrace($"Finished applying {rule.Name}.");
          }
          //Otherwise, do nothing... this is expected
        }
        catch (EngineHaltException)
        {
          //Cancel and throw.
          //cancellationTokenSource.Cancel();
          throw;
        }
      }
      else
      {
        Logger.LogTrace($"Rule {rule.Name} does not apply.");
      }
    }
    catch (Exception e)
    {
      throw new EngineHaltException("Engine halted due to uncaught exception.", e)
      {
        Context = context,
        Input = input,
        Output = null,
        Rule = rule
      };
    }
  }

  private async Task ApplySerial(T input, IEngineContext context, CancellationToken token)
  {
    foreach (var set in _rules)
      foreach (var rule in set)
        await ApplyRule(context, rule, input, token).ConfigureAwait(false);
  }
  private async Task ApplyParallel(T input, IEngineContext context, CancellationToken token)
  {
    foreach (var set in _rules)
      await Task.WhenAll(
          set.Select(r => Task.Run(() => ApplyRule(context, r, input, token)))
      ).ConfigureAwait(false);
  }

  private async Task ApplyManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context, CancellationToken token)
  {
    foreach (var input in inputs) await ApplyAsync(input, context, token).ConfigureAwait(false);
  }

  private Task ApplyParallelManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context, CancellationToken token)
      => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplySerial(i, context, token))));

  private async Task ApplyManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context, CancellationToken token)
  {
    foreach (var input in inputs) await ApplyParallel(input, context, token).ConfigureAwait(false);
  }

  private Task ApplyParallelManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context, CancellationToken token)
      => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyParallel(i, context, token))));

  internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;
}