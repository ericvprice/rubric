using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Engines.Probabilistic.Default;
using Rubric.Rules.Probabilistic.Async;
using Rubric.Rulesets.Probabilistic.Async;

namespace Rubric.Engines.Probabilistic.Async.Default;

public class RuleEngine<T> : BaseProbabilisticRuleEngine, IRuleEngine<T>
    where T : class
{

  #region Fields

  private readonly IRule<T>[][] _rules;

  #endregion

  #region Constructors
  /// <summary>
  ///     Default public constructor.
  /// </summary>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger.</param>
  public RuleEngine(IRuleset<T> rules,
                      bool isParallel = false,
                      IExceptionHandler exceptionHandler = null,
                      ILogger logger = null)
        : this(rules.Rules, isParallel, exceptionHandler, logger) { }

  /// <summary>
  ///     Default public constructor.
  /// </summary>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="isParallel">Whether to execute rules in parallel.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger.</param>
  public RuleEngine(
        IEnumerable<IRule<T>> rules,
        bool isParallel = false,
        IExceptionHandler exceptionHandler = null,
        ILogger logger = null
    )
    {
        rules ??= Enumerable.Empty<IRule<T>>();
        _rules = rules.ResolveDependencies()
                        .Select(e => e.ToArray())
                        .ToArray();
        IsParallel = isParallel;
        Logger = logger ?? NullLogger.Instance;
        ExceptionHandler = exceptionHandler ?? ExceptionHandlers.Rethrow;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public bool IsParallel { get; internal set; }

    /// <inheritdoc />
    public override bool IsAsync => true;

    /// <inheritdoc />
    public override Type InputType => typeof(T);

    /// <inheritdoc />
    public override Type OutputType => typeof(T);

    /// <inheritdoc />
    public IEnumerable<IRule<T>> Rules => _rules.SelectMany(_ => _);

    #endregion

    #region Public Methods

    /// <inheritdoc />
    public async Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default)
    {
        context = Reset(context);
        try
        {
            await ApplyItemAsync(input, context, token);
        }
        catch (EngineException) { }
    }

    /// <inheritdoc />
    public async Task ApplyAsync(
      IEnumerable<T> inputs,
      IEngineContext ctx = null,
      bool parallelizeInputs = false,
      CancellationToken token = default)
    {
        ctx = Reset(ctx);
        using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
            try
            {
                if (parallelizeInputs)
                    await ApplyManyParallelAsync(ctx, inputs, token);
                else
                    await ApplyManySerialAsync(inputs, ctx, token);
            }
            catch (EngineHaltException) { }
    }

    public async Task ApplyAsync(
      IAsyncEnumerable<T> inputs,
      IEngineContext ctx = null,
      CancellationToken token = default)
    {
        ctx = Reset(ctx);
        using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
            try
            {
                await ApplyManyAsync(inputs, ctx, token);
            }
            catch (EngineHaltException) { }
    }

    #endregion

    #region Private Methods

    private Task ApplyItemAsync(T input, IEngineContext context = null, CancellationToken token = default)
    {
        using (Logger.BeginScope("Input", input))
            return IsParallel
              ? ApplyParallel(context, input, token)
              : ApplySerial(context, input, token);
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
        context[EngineContextExtensions.ENGINE_KEY] = this;
        context[EngineContextExtensions.TRACE_ID_KEY] = Guid.NewGuid().ToString();
        context[ProbabilisiticEngineExtensions.RANDOM_KEY] = Random;
        return context;
    }

    private async Task ApplyManyAsync(IAsyncEnumerable<T> inputs, IEngineContext context, CancellationToken t)
    {
        await foreach (var input in inputs.WithCancellation(t))
        {
            await ApplyItemAsync(input, context, t).ConfigureAwait(false);
        }
    }

    private async Task ApplyManySerialAsync(IEnumerable<T> inputs, IEngineContext context, CancellationToken t)
    {
        foreach (var input in inputs)
        {
            t.ThrowIfCancellationRequested();
            await ApplyItemAsync(input, context, t);
        }
    }

    private Task ApplyManyParallelAsync(IEngineContext ctx, IEnumerable<T> inputs, CancellationToken t)
        => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyItemAsync(i, ctx, t), t)));

    #endregion

}