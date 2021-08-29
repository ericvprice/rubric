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
    try
    {
      context = Reset(context);
      if (IsParallel)
        await ApplyParallel(context, input, token);
      else
        await ApplySerial(context, input, token);
    }
    catch (EngineHaltException) { }
  }

  public async Task ApplyAsync(IEnumerable<T> inputs, IEngineContext ctx = null, bool parallelizeInputs = false, CancellationToken token = default)
  {
    Reset(ctx);
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
    
  private async Task ApplySerial(IEngineContext ctx, T i,CancellationToken t)
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
      try
      {
        await Parallelize(ctx, set, i, t).ConfigureAwait(false);
      }
      //TODO Handle aggregate... how?
      catch (ItemHaltException)
      {
        return;
      }
    }
  }

  private Task Parallelize(IEngineContext ctx, IEnumerable<IAsyncRule<T>> rules, T i, CancellationToken t)
  {
    var cts = CancellationTokenSource.CreateLinkedTokenSource(t);
    t = cts.Token;
    return Task.WhenAll(
      rules.Select(
        r => Task.Run(async () => {
                        try { await this.ApplyAsyncPreRule(ctx, r, i, t); } 
                        catch (Exception) { cts.Cancel(); throw; }
                      },t)));
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
        await ApplyAsync(input, context, t).ConfigureAwait(false);
      }
      catch (ItemHaltException)
      {
        continue;
      }
    }
  }

  private async Task ApplyManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context, CancellationToken t)
  {
    foreach (var input in inputs)
      try
      {
        await ApplyParallel(context, input, t).ConfigureAwait(false);
      }
      catch (ItemHaltException)
      {
        continue;
      }
  }

  private Task ApplyParallelManyAsyncParallel(IEngineContext ctx, IEnumerable<T> inputs, CancellationToken t)
      => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyParallel(ctx, i, t))));

  private Task ApplyParallelManyAsyncSerial(IEnumerable<T> inputs, IEngineContext ctx, CancellationToken t)
      => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplySerial(ctx, i, t))));

  #endregion
}