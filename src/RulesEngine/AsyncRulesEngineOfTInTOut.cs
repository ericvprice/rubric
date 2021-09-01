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

  #region Constructors

  /// <summary>
  ///     Convenience ruleset constructor.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
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
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// /// <param name="logger">An optional logger.</param>
  public AsyncRulesEngine(
      Ruleset<TIn, TOut> ruleSet,
      bool isParallel = false,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null
  ) : this(
      ruleSet.PreRules,
      rulesFull: ruleSet.Rules,
      postRulesFull: ruleSet.PostRules,
      isParallel: isParallel,
      exceptionHandler: exceptionHandler,
      logger: logger)
  { }

  /// <summary>
  ///     Construct an async rule engine based on individual collections of async rules.
  /// </summary>
  /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">A logger.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
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
  ///     Construct an async rule engine based on individual collections of async and sync rules.
  /// </summary>
  /// <param name="preRulesFull">Collection of synchronous preprocessing rules.</param>
  /// <param name="asyncPreRulesFull">Collection of asynchronous preprocessing rules.</param>
  /// <param name="rulesFull">Collection of synchronous processing rules.</param>
  /// <param name="asyncRulesFull">Collection of asynchronous processing rules.</param>
  /// <param name="postRulesFull">Collection of synchronous postprocessing rules.</param>
  /// <param name="asyncPostRulesFull">Collection of asynchronous postprocessing rules.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
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

  #endregion

  #region Properties

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

  #endregion

  ///<inheritdoc/>
  public async Task ApplyAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    context = Reset(context);
    try
    {
      await ApplyItemAsync(input, output, context, token);
      await ApplyPostAsync(output, context, token);
    }
    catch (EngineException) { }
  }

  ///<inheritdoc/>
  public async Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    context = Reset(context);
    try
    {
      await ApplyManyAsyncSerial(inputs, output, context, token);
      await ApplyPostAsync(output, context, token);
    }
    catch (EngineException) { }
  }

  ///<inheritdoc/>
  public async Task ApplyParallelAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context, CancellationToken token = default)
  {
    context = Reset(context);
    try
    {
      await ApplyManyAsyncParallel(inputs, output, context, token);
      await ApplyPostAsync(output, context, token);
    }
    catch (EngineException) { }
  }

  ///<inheritdoc/>
  public async Task ApplyAsync(IAsyncEnumerable<TIn> inputStream, TOut output, IEngineContext context, CancellationToken token = default)
  {
    try
    {
      await foreach (var input in inputStream)
      {
        await ApplyItemAsync(input, output, context, token);
      }
      await ApplyPostAsync(output, context, token);
    }
    catch (EngineException) { }
  }

  /// <summary>
  ///   Process a single item.
  /// </summary>
  /// <param name="input">The input.</param>
  /// <param name="output">The output.</param>
  /// <param name="context">The context.</param>
  /// <param name="token">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private async Task ApplyItemAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    if (IsParallel)
      await ApplyItemParallel(context, input, output, token);
    else
      await ApplyItemSerial(context, input, output, token);
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
      await ParallelizePre(ctx, set, i, t);
    }
    foreach (var set in _rules)
    {
      t.ThrowIfCancellationRequested();
      await Parallelize(ctx, set, i, o, t);
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
    if (IsParallel)
    {
      foreach (var set in _postRules)
      {
        t.ThrowIfCancellationRequested();
        await ParallelizePost(ctx, set, o, t);
      }
    }
    else
    {
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
      try
      {
        foreach (var set in _preRules)
          foreach (var pre in set)
          {
            t.ThrowIfCancellationRequested();
            await this.ApplyAsyncPreRule(ctx, pre, input, t);
          }
        foreach (var set in _rules)
          foreach (var rule in set)
          {
            t.ThrowIfCancellationRequested();
            await this.ApplyAsyncRule(ctx, rule, input, output, t);
          }
      }
      catch (ItemHaltException)
      {
        continue;
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
    await ParallelizeInputs(ctx, inputs, o, t);
    await ApplyPostAsync(o, ctx, t);
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
    t = cts.Token;
    return Task.WhenAll(
      inputs.Select(
        i => Task.Run(async () =>
        {
          t.ThrowIfCancellationRequested();
          try
          {
            await ApplyItemAsync(i, o, ctx, t);
          }
          catch (ItemHaltException) { }
          catch (Exception) { cts.Cancel(); throw; }
        }, t)));
  }

  /// <summary>
  ///   Parallelize a set of preprocessing rules.  Cancel other rules when any rule throws.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="rules">The rules to parallelize.</param>
  /// <param name="i">The input being processed.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task ParallelizePre(IEngineContext ctx, IEnumerable<IAsyncRule<TIn>> rules, TIn i, CancellationToken t)
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

  /// <summary>
  ///   Parallelize a set of rules.  Cancel other rules when any rule throws.
  /// </summary>
  /// <param name="ctx">The execution context.</param>
  /// <param name="rules">The rules to parallelize.</param>
  /// <param name="i">The input being processed.</param>
  /// <param name="o">The output being processed.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
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

  /// <summary>
  ///   Parallelize a set of postprocessing rules.  Cancel other rules when any rule throws.
  /// </summary>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <param name="ctx">The execution context.</param>
  /// <param name="rules">The rules to parallelize.</param>
  /// <param name="o">The input being processed.</param>
  /// <param name="t">The cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  private Task ParallelizePost(IEngineContext ctx, IEnumerable<IAsyncRule<TOut>> rules, TOut o, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    t = cts.Token;
    return Task.WhenAll(
      rules.Select(
        r => Task.Run(async () =>
        {
          try { await this.ApplyAsyncPreRule(ctx, r, o, t); }
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