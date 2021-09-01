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

  #region Constructors

  /// <summary>
  ///     Convenience ruleset constructor.
  /// </summary>
  /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="handler">An exception handler.</param>
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
  /// <param name="handler">An exception handler.</param>
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
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">A logger.</param>
  public AsyncRulesEngine(
      IEnumerable<IAsyncRule<T>> asyncRules,
      bool isParallel = false,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null
  ) : this(null, asyncRules, isParallel, exceptionHandler, logger) { }


  /// <summary>
  ///     Full constructor.
  /// </summary>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="handler">An optional exception handler.</param>
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

  #endregion

  #region Properties

  public bool IsParallel { get; internal set; }

  public bool IsAsync => true;

  /// <inheritdoc />
  public Type InputType => typeof(T);

  /// <inheritdoc />
  public Type OutputType => typeof(T);

  public IEnumerable<IAsyncRule<T>> Rules => _rules.SelectMany(_ => _);

  public ILogger Logger { get; }

  public IExceptionHandler ExceptionHandler { get; }

  public EngineException LastException { get; set; }

  #endregion

  #region Methods
  public async Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default)
  {
    context = Reset(context);
    try
    {
      await ApplyItemAsync(input, context, token);
    }
    catch (EngineException) { }
  }

  private Task ApplyItemAsync(T input, IEngineContext context = null, CancellationToken token = default)
    => IsParallel ? ApplyParallel(context, input, token) : ApplySerial(context, input, token);

  public async Task ApplyAsync(
    IEnumerable<T> inputs, 
    IEngineContext ctx = null, 
    bool parallelizeInputs = false, 
    CancellationToken token = default)
  {
    ctx = Reset(ctx);
    try
    {
      if (IsParallel)
        if (parallelizeInputs)
          await ApplyParallelManyAsyncParallel(ctx, inputs, token);
        else
          await ApplyManyAsyncParallel(inputs, ctx, token);
      else
        if (parallelizeInputs)
        await ApplyParallelManyAsyncSerial(inputs, ctx, token);
      else
        await ApplyManyAsyncSerial(inputs, ctx, token);
    }
    catch (EngineHaltException) { }
  }

  public async Task ApplyAsync(
    IAsyncEnumerable<T> inputs,
    IEngineContext ctx = null,
    CancellationToken token = default)
  {
    ctx = Reset(ctx);
    try
    {
      if (IsParallel)
        await ApplyManyAsyncParallel(inputs, ctx, token);
      else
        await ApplyManyAsyncSerial(inputs, ctx, token);
    }
    catch (EngineHaltException) { }
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

  private Task Parallelize(IEngineContext ctx, IEnumerable<IAsyncRule<T>> rules, T i, CancellationToken t)
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

  private async Task ApplyManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    foreach (var input in inputs)
    {
      try
      {
        await ApplyItemAsync(input, context, t).ConfigureAwait(false);
      }
      catch (EngineHaltException)
      {
        break;
      }
    }
  }

  private async Task ApplyManyAsyncSerial(IAsyncEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    await foreach (var input in inputs)
    {
      try
      {
        await ApplyItemAsync(input, context, t).ConfigureAwait(false);
      }
      catch (EngineHaltException)
      {
        break;
      }
    }
  }

  private async Task ApplyManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    foreach (var input in inputs)
    {
      try
      {
        t.ThrowIfCancellationRequested();
        await ApplyParallel(context, input, t).ConfigureAwait(false);
      }
      catch (EngineHaltException)
      {
        break;
      }
    }
  }

  private async Task ApplyManyAsyncParallel(IAsyncEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    await foreach (var input in inputs)
    {
      try
      {
        t.ThrowIfCancellationRequested();
        await ApplyParallel(context, input, t).ConfigureAwait(false);
      }
      catch (EngineHaltException)
      {
        break;
      }
    }
  }

  private Task ApplyParallelManyAsyncParallel(IEngineContext ctx, IEnumerable<T> inputs, CancellationToken t)
      => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyParallel(ctx, i, t))));

  private Task ApplyParallelManyAsyncSerial(IEnumerable<T> inputs, IEngineContext ctx, CancellationToken t)
      => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplySerial(ctx, i, t))));

  #endregion
}