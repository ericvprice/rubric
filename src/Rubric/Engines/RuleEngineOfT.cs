using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Rules;
using Rubric.Rulesets;

namespace Rubric.Engines;

public class RuleEngine<T> : BaseRuleEngine, IRuleEngine<T>
    where T : class
{

  #region Fields

  private readonly IRule<T>[][] _rules;

  #endregion

  #region Constructors

  public RuleEngine(IRuleset<T> ruleset,
                    IExceptionHandler uncaughtExceptionHandler = null,
                    ILogger logger = null)
      : this(ruleset.Rules, uncaughtExceptionHandler, logger) { }

  /// <summary>
  ///     Default public constructor.
  /// </summary>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger.</param>
  public RuleEngine(
      IEnumerable<IRule<T>> rules,
      IExceptionHandler exceptionHandler = null,
      ILogger logger = null
  )
  {
    rules ??= Enumerable.Empty<IRule<T>>();
    _rules = rules.ResolveDependencies()
                    .Select(e => e.ToArray())
                    .ToArray();
    Logger = logger ?? NullLogger.Instance;
    ExceptionHandler = exceptionHandler ?? ExceptionHandlers.Rethrow;
  }

  #endregion

  #region Properties

  public IEnumerable<IRule<T>> Rules
      => _rules.SelectMany(_ => _);

  /// <inheritdoc />
  public override bool IsAsync => false;

  /// <inheritdoc />
  public override Type InputType => typeof(T);

  /// <inheritdoc />
  public override Type OutputType => typeof(T);

  #endregion

  #region Methods

  ///<inheritdoc/>
  public void Apply(T input, IEngineContext context = null)
  {
    var ctx = Reset(context);
    using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
      try
      {
        ApplyItem(input, ctx);
      }
      catch (EngineHaltException) { }
  }

  ///<inheritdoc/>
  public void Apply(IEnumerable<T> inputs, IEngineContext context = null)
  {
    var ctx = Reset(context);
    using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
      foreach (var input in inputs)
      {
        try
        {
          ApplyItem(input, ctx);
        }
        catch (EngineHaltException)
        {
          break;
        }
      }
  }

  private void ApplyItem(T input, IEngineContext ctx)
  {
    using (Logger.BeginScope("Input", input))
      foreach (var set in _rules)
        foreach (var rule in set)
          try
          {
            this.ApplyPreRule(ctx, rule, input);
          }
          catch (ItemHaltException)
          {
            return;
          }
  }

  private IEngineContext Reset(IEngineContext ctx)
  {
    ctx ??= new EngineContext();
    ctx[EngineContextExtensions.ENGINE_KEY] = this;
    ctx[EngineContextExtensions.TRACE_ID_KEY] = Guid.NewGuid().ToString();
    return ctx;
  }

  #endregion

}