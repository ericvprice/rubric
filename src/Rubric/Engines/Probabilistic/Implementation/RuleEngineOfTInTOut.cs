using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Rules.Probabilistic;
using Rubric.Rulesets.Probabilistic;

namespace Rubric.Engines.Probabilistic.Implementation;

/// <summary>
///   Default implementation of a rule engine.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public class RuleEngine<TIn, TOut> : BaseProbabilisticRuleEngine, IRuleEngine<TIn, TOut>
  where TIn : class
  where TOut : class
{
#region Fields

  private readonly IRule<TOut>[][] _postprocessingRules;

  private readonly IRule<TIn>[][] _preprocessingRules;

  private readonly IRule<TIn, TOut>[][] _rules;

#endregion

#region Constructors

  /// <summary>
  ///   Construct a rule engine from a ruleset.
  /// </summary>
  /// <param name="ruleset">A collection of various rules</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger</param>
  public RuleEngine(IRuleset<TIn, TOut> ruleset,
                    IExceptionHandler exceptionHandler = null,
                    ILogger logger = null)
    : this(ruleset?.PreRules, ruleset?.Rules, ruleset?.PostRules, exceptionHandler, logger) { }

  /// <summary>
  ///   Default public constructor.
  /// </summary>
  /// <param name="preprocessingRules">Collection of synchronous preprocessing rules.</param>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="postprocessingRules">Collection of synchronous postprocessing rules.</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger.</param>
  public RuleEngine(
    IEnumerable<IRule<TIn>> preprocessingRules,
    IEnumerable<IRule<TIn, TOut>> rules,
    IEnumerable<IRule<TOut>> postprocessingRules,
    IExceptionHandler exceptionHandler = null,
    ILogger logger = null
  )
  {
    preprocessingRules ??= Enumerable.Empty<IRule<TIn>>();
    _preprocessingRules =
      preprocessingRules.ResolveDependencies()
                        .Select(e => e.ToArray())
                        .ToArray();
    postprocessingRules ??= Enumerable.Empty<IRule<TOut>>();
    _postprocessingRules
      = postprocessingRules.ResolveDependencies()
                           .Select(e => e.ToArray())
                           .ToArray();
    rules ??= Enumerable.Empty<IRule<TIn, TOut>>();
    _rules
      = rules.ResolveDependencies()
             .Select(e => e.ToArray())
             .ToArray();
    ExceptionHandler = exceptionHandler ?? ExceptionHandlers.Rethrow;
    Logger = logger ?? NullLogger.Instance;
  }

#endregion

#region Properties

  /// <inheritdoc />
  public IEnumerable<IRule<TIn>> PreRules
    => _preprocessingRules.SelectMany(r => r);

  /// <inheritdoc />
  public IEnumerable<IRule<TIn, TOut>> Rules
    => _rules.SelectMany(r => r);

  /// <inheritdoc />
  public IEnumerable<IRule<TOut>> PostRules
    => _postprocessingRules.SelectMany(r => r);

  /// <inheritdoc />
  public override bool IsAsync => false;

  /// <inheritdoc />
  public override Type InputType => typeof(TIn);

  /// <inheritdoc />
  public override Type OutputType => typeof(TOut);

#endregion

#region Public Methods

  /// <inheritdoc />
  public void Apply(TIn input, TOut output, IEngineContext context = null)
  {
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        ApplyItem(input, output, context);
        ApplyPostRules(output, context);
      }
      catch (EngineHaltException) { }
    }
  }

  /// <inheritdoc />
  public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
  {
    if (inputs == null) throw new ArgumentNullException(nameof(inputs));
    var ctx = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", ctx.GetTraceId()))
    {
      try
      {
        foreach (var input in inputs)
          ApplyItem(input, output, ctx);
        ApplyPostRules(output, ctx);
      }
      catch (EngineHaltException) { }
    }
  }

#endregion

#region Nonpublic Methods

  private void ApplyItem(TIn input, TOut output, IEngineContext ctx)
  {
    using (Logger.BeginScope("Input", input))
    {
      try
      {
        foreach (var set in _preprocessingRules)
          foreach (var rule in set)
            this.ApplyPreRule(ctx, rule, input);
        foreach (var set in _rules)
          foreach (var rule in set)
            this.ApplyRule(ctx, rule, input, output);
      }
      catch (ItemHaltException) { }
    }
  }

  private void ApplyPostRules(TOut output, IEngineContext ctx)
  {
    using (Logger.BeginScope("Output", output))
    {
      try
      {
        foreach (var set in _postprocessingRules)
          foreach (var rule in set)
            this.ApplyPostRule(ctx, rule, output);
      }
      catch (ItemHaltException) { }
    }
  }

#endregion
}