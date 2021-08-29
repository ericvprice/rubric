using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine;

public class AsyncRulesEngine<TIn, TOut> : IAsyncRulesEngine<TIn, TOut>
    where TIn : class
    where TOut : class
{
  /// <summary>
  ///     Ordered and parallelized postprocessing rules
  /// </summary>
  private readonly IAsyncRule<TOut>[][] _postRules;

  /// <summary>
  ///     Ordered and parallelized pre processing rules
  /// </summary>
  private readonly IAsyncRule<TIn>[][] _preRules;

  /// <summary>
  ///     Ordered and parallelized processing rules
  /// </summary>
  private readonly IAsyncRule<TIn, TOut>[][] _rules;

  /// <summary>
  ///     Convenience ruleset constructor.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      AsyncRuleset<TIn, TOut> ruleSet,
      bool isParallel = false,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null
  ) : this(
      asyncPreRulesFull: ruleSet.AsyncPreRules,
      asyncRulesFull: ruleSet.AsyncRules,
      asyncPostRulesFull: ruleSet.AsyncPostRules,
      isParallel: isParallel,
      exceptionHandler: exceptionHandler,
      logger: logger)
  { }

  /// <summary>
  ///     Create a rule engine based on a synchronous ruleset.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous rules.</param>
  /// <param name="isParallel">Optionally execute in parallel.  False by default.</param>
  /// <param name="logger">An optional logger.</param>
  public AsyncRulesEngine(
      Ruleset<TIn, TOut> ruleSet,
      bool isParallel = false,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null
  ) : this(
      preRulesFull: ruleSet.PreRules,
      rulesFull: ruleSet.Rules,
      postRulesFull: ruleSet.PostRules,
      isParallel: isParallel,
      exceptionHandler: exceptionHandler,
      logger: logger)
  { }

  /// <summary>
  ///     Construct an async rule engine based on inidvidual collections of async rules.
  /// </summary>
  /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      IEnumerable<IAsyncRule<TIn>> asyncPreRules,
      IEnumerable<IAsyncRule<TIn, TOut>> asyncRules,
      IEnumerable<IAsyncRule<TOut>> asyncPostRules,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null,
      bool isParallel = false
  ) : this(asyncPreRulesFull: asyncPreRules,
              asyncRulesFull: asyncRules,
              asyncPostRulesFull: asyncPostRules,
              isParallel: isParallel,
              logger: logger,
              exceptionHandler: exceptionHandler)
  { }


  /// <summary>
  ///     Construct an async rule engine based on inidvidual collections of async and sync rules.
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
      IEnumerable<IRule<TIn>> preRulesFull = null,
      IEnumerable<IAsyncRule<TIn>> asyncPreRulesFull = null,
      IEnumerable<IRule<TIn, TOut>> rulesFull = null,
      IEnumerable<IAsyncRule<TIn, TOut>> asyncRulesFull = null,
      IEnumerable<IRule<TOut>> postRulesFull = null,
      IEnumerable<IAsyncRule<TOut>> asyncPostRulesFull = null,
      bool isParallel = false,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null
  )
  {
    IsParallel = isParallel;
    preRulesFull ??= Enumerable.Empty<IRule<TIn>>();
    asyncPreRulesFull ??= Enumerable.Empty<IAsyncRule<TIn>>();
    rulesFull ??= Enumerable.Empty<IRule<TIn, TOut>>();
    asyncRulesFull ??= Enumerable.Empty<IAsyncRule<TIn, TOut>>();
    postRulesFull ??= Enumerable.Empty<IRule<TOut>>();
    asyncPostRulesFull ??= Enumerable.Empty<IAsyncRule<TOut>>();
    _preRules = preRulesFull.Select(r => r.WrapAsync())
                        .Concat(asyncPreRulesFull)
                        .ResolveDependencies()
                        .Select(e => e.ToArray())
                        .ToArray();
    _postRules = postRulesFull.Select(r => r.WrapAsync())
                            .Concat(asyncPostRulesFull)
                            .ResolveDependencies()
                            .Select(e => e.ToArray())
                            .ToArray();
    _rules = rulesFull.Select(r => r.WrapAsync())
                    .Concat(asyncRulesFull)
                    .ResolveDependencies()
                    .Select(e => e.ToArray())
                    .ToArray();
    Logger = logger ?? NullLogger.Instance;
    ExceptionHandler = exceptionHandler ?? ExceptionHandlers.Throw;
  }

  public bool IsParallel { get; internal set; }

  public bool IsAsync => true;

  /// <inheritdoc />
  public Type InputType => typeof(TIn);

  /// <inheritdoc />
  public Type OutputType => typeof(TOut);

  public IExceptionHandler ExceptionHandler { get; }

  public IEnumerable<IAsyncRule<TIn>> PreRules => _preRules.SelectMany(_ => _);

  public IEnumerable<IAsyncRule<TIn, TOut>> Rules => _rules.SelectMany(_ => _);

  public IEnumerable<IAsyncRule<TOut>> PostRules => _postRules.SelectMany(_ => _);

  public ILogger Logger { get; }

  public EngineException LastException { get; set; }

  public async Task ApplyAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    context = Reset(context);
    try
    {
      await ApplyItemAsync(input, output, context, token);
    }
    catch (EngineException) { }
  }

  private Task ApplyItemAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default)
  => IsParallel ? ApplyParallel(context, input, output, token) : ApplySerial(context, input, output, token);


  public async Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    context = Reset(context);
    try
    {
      if (IsParallel)
        await ApplyManyAsyncParallel(inputs, output, context, token);
      else
        await ApplyManyAsyncSerial(inputs, output, context, token);
    } catch (EngineException) { }
  }


  private async Task ApplySerial(IEngineContext ctx, TIn i, TOut o, CancellationToken t)
  {
      foreach (var set in _preRules)
        foreach (var rule in set)
          await this.ApplyAsyncPreRule(ctx, rule, i, t).ConfigureAwait(false);
      foreach (var set in _rules)
        foreach (var rule in set)
          await this.ApplyAsyncRule(ctx, rule, i, o, t).ConfigureAwait(false);
    foreach (var set in _postRules)
      foreach (var rule in set)
        await this.ApplyAsyncPostRule(ctx, rule, o, t).ConfigureAwait(false);
  }

  private async Task ApplyParallel(IEngineContext ctx, TIn i, TOut o, CancellationToken t)
  {
    try
    {
      foreach (var set in _preRules)
        foreach (var pre in set)
          await ParallelizePre(ctx, set, i, t);
      foreach (var set in _rules)
        await Parallelize(ctx, set, i, o, t);
    }
    catch (ItemHaltException)
    {
      return;
    }
    foreach (var set in _postRules)
      await ParallelizePost(ctx, set, o, t);
  }

  private async Task ApplyManyAsyncSerial(IEnumerable<TIn> inputs, TOut output, IEngineContext ctx, CancellationToken t)
  {
    foreach (var input in inputs)
    {
      try
      {
        foreach (var set in _preRules)
          foreach(var pre in set)
            await this.ApplyAsyncPreRule(ctx, pre, input, t);
        foreach (var set in _rules)
            foreach(var rule in set)
          await this.ApplyAsyncRule(ctx, rule, input, output, t);
      } 
      catch(ItemHaltException)
      {
        continue;
      }
    }
    foreach (var set in _postRules)
      await ParallelizePost(ctx, set, output, t);
  }

  private async Task ApplyManyAsyncParallel(IEnumerable<TIn> inputs, TOut output, IEngineContext ctx, CancellationToken t)
  {
    foreach (var input in inputs)
    {
      try
      {
        foreach (var set in _preRules)
          foreach (var pre in set)
            await ParallelizePre(ctx, set, input, t);
        foreach (var set in _rules)
          await Parallelize(ctx, set, input, output, t);
      }
      catch (ItemHaltException)
      {
        continue;
      }
    }
    foreach (var set in _postRules)
      await ParallelizePost(ctx, set, output, t);
  }

  private Task ParallelizePre<T>(IEngineContext ctx, IEnumerable<IAsyncRule<T>> rules, T i, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    t = cts.Token;
    return Task.WhenAll(
      rules.Select(
        r => Task.Run(async () =>
        {
          try { await this.ApplyAsyncPreRule(ctx, r, i, t); }
          catch (Exception) { cts.Cancel(); throw; }
        }, t)));
  }

  private Task Parallelize(IEngineContext ctx, IEnumerable<IAsyncRule<TIn, TOut>> rules, TIn i, TOut o, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    t = cts.Token;
    return Task.WhenAll(
      rules.Select(
        r => Task.Run(async () =>
        {
          try { await this.ApplyAsyncRule(ctx, r, i, o, t); }
          catch (Exception) { cts.Cancel(); throw; }
        }, t)));
  }
  private Task ParallelizePost<T>(IEngineContext ctx, IEnumerable<IAsyncRule<T>> rules, T i, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    t = cts.Token;
    return Task.WhenAll(
      rules.Select(
        r => Task.Run(async () =>
        {
          try { await this.ApplyAsyncPreRule(ctx, r, i, t); }
          catch (Exception) { cts.Cancel(); throw; }
        }, t)));
  }
  private IEngineContext Reset(IEngineContext context)
  {
    context ??= new EngineContext();
    SetupContext(context);
    LastException = null;
    return context;
  }

  internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;

}