using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Rules;
using Rubric.Rulesets;

namespace Rubric.Engines.Implementation;

/// <inheritdoc cref="IRuleEngine{TIn, TOut}" />
public class RuleEngine<TIn, TOut> : BaseRuleEngine, IRuleEngine<TIn, TOut>
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
      finally
      {
        context.ClearAllCaches();
      }
    }
  }

  /// <inheritdoc />
  public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
  {
    if (inputs == null) throw new ArgumentNullException(nameof(inputs));
    context = SetupContext(context);
    using (Logger.BeginScope("ExecutionId", context.GetTraceId()))
    {
      try
      {
        foreach (var input in inputs)
        {
          try
          {
            ApplyItem(input, output, context);
          }
          finally
          {
            context.ClearInputPredicateCache();
          }
        }
        ApplyPostRules(output, context);
      }
      catch (EngineHaltException) { }
      finally
      {
        context.ClearAllCaches();
      }
    }
  }

  #endregion

  #region Nonpublic Methods

  /// <summary>
  ///   Apply preprocessing rules and rules for an input.  Catch any item halt exceptions.
  /// </summary>
  /// <param name="input">The input being processed.</param>
  /// <param name="output">The output.</param>
  /// <param name="ctx">The current execution context.</param>
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

  /// <summary>
  ///   Apply postprocessing rules to the output object.  Catch any item halt exceptions.
  /// </summary>
  /// <param name="output">The output.</param>
  /// <param name="ctx">The current execution context.</param>
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