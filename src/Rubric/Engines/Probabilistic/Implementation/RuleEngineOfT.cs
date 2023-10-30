using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Rules.Probabilistic;
using Rubric.Rulesets.Probabilistic;

namespace Rubric.Engines.Probabilistic.Implementation;

/// <inheritdoc cref="IRuleEngine{T}" />
public class RuleEngine<T> : BaseProbabilisticRuleEngine, IRuleEngine<T>
  where T : class
{
#region Fields

  private readonly IRule<T>[][] _rules;

#endregion

#region Constructors

  /// <summary>
  ///   Ruleset constructor.
  /// </summary>
  /// <param name="ruleset">The ruleset to use.</param>
  /// <param name="uncaughtExceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger.</param>
  public RuleEngine(IRuleset<T> ruleset,
                    IExceptionHandler uncaughtExceptionHandler = null,
                    ILogger logger = null)
    : this(ruleset?.Rules, uncaughtExceptionHandler, logger) { }

  /// <summary>
  ///   Default public constructor.
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

  /// <inheritdoc />
  public IEnumerable<IRule<T>> Rules
    => _rules.SelectMany(r => r);

  /// <inheritdoc />
  public override bool IsAsync => false;

  /// <inheritdoc />
  public override Type InputType => typeof(T);

  /// <inheritdoc />
  public override Type OutputType => typeof(T);

#endregion

#region Methods

  /// <inheritdoc />
  public void Apply(T input, IEngineContext context = null)
  {
    if(input == null) throw new ArgumentNullException(nameof(input));
    var ctx = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
    {
      try
      {
        ApplyItem(input, ctx);
      }
      catch (EngineHaltException) { }
      finally
      {
        ctx.ClearExecutionPredicateCache();
      }
    }
  }

  /// <inheritdoc />
  public void Apply(IEnumerable<T> inputs, IEngineContext context = null)
  {
    if (inputs == null) throw new ArgumentNullException(nameof(inputs));
    var ctx = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
    {
      try
      {
        foreach (var input in inputs)
        {
          ApplyItem(input, ctx);
        }
      }
      catch (EngineHaltException) { }
      finally
      {
        ctx.ClearExecutionPredicateCache();
      }
    }
  }

  private void ApplyItem(T input, IEngineContext ctx)
  {
    using (Logger.BeginScope("Input", input))
    {
      try
      {
        foreach (var set in _rules)
          foreach (var rule in set)
            try
            {
              this.ApplyPreRule(ctx, rule, input);
            }
            catch (ItemHaltException)
            {
              break;
            }
      }
      finally
      {
        ctx.ClearInputPredicateCache();
      }
    }
  }

#endregion
}