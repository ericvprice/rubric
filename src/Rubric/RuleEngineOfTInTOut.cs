using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Dependency;
using Rubric.Rules;
using Rubric.Rulesets;

namespace Rubric;

public class RuleEngine<TIn, TOut> : IRuleEngine<TIn, TOut>
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
  ///     Construct a rule engine from a ruleset.
  /// </summary>
  /// <param name="ruleset">A collection of various rules</param>
  /// <param name="exceptionHandler">An optional exception handler.</param>
  /// <param name="logger">An optional logger</param>
  public RuleEngine(Ruleset<TIn, TOut> ruleset,
                    IExceptionHandler exceptionHandler = null,
                    ILogger logger = null)
      : this(ruleset.PreRules, ruleset.Rules, ruleset.PostRules, exceptionHandler, logger) { }

  /// <summary>
  ///     Default public constructor.
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

  public IEnumerable<IRule<TIn>> PreRules
      => _preprocessingRules.SelectMany(_ => _);

  public IEnumerable<IRule<TIn, TOut>> Rules
      => _rules.SelectMany(_ => _);

  public IEnumerable<IRule<TOut>> PostRules
      => _postprocessingRules.SelectMany(_ => _);

  /// <inheritdoc />
  public ILogger Logger { get; }

  /// <inheritdoc />
  public bool IsAsync => false;

  /// <inheritdoc />
  public Type InputType => typeof(TIn);

  /// <inheritdoc />
  public Type OutputType => typeof(TOut);

  public IExceptionHandler ExceptionHandler { get; }

  public EngineException LastException { get; set; }

  #endregion

  #region Public Methods

  ///<inheritdoc/>
  public void Apply(TIn input, TOut output, IEngineContext context = null)
  {
    context = SetupContext(context);
    try
    {
      ApplyItem(input, output, context);
      foreach (var set in _postprocessingRules)
        foreach (var rule in set)
          this.ApplyPostRule(context, rule, output);
    }
    catch (EngineHaltException)
    {
      return;
    }
  }

  ///<inheritdoc/>
  public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
  {
    var ctx = context ?? new EngineContext();
    try
    {
      foreach (var input in inputs)
        ApplyItem(input, output, ctx);
      foreach (var set in _postprocessingRules)
        foreach (var rule in set)
          this.ApplyPostRule(ctx, rule, output);
    }
    catch (EngineHaltException)
    {
      return;
    }
  }

  #endregion

  #region Nonpublic Methods

  private void ApplyItem(TIn input, TOut output, IEngineContext ctx)
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
    catch (ItemHaltException)
    {
      return;
    }
  }

  internal IEngineContext SetupContext(IEngineContext ctx)
  {
    ctx ??= new EngineContext();
    LastException = null;
    ctx[EngineContextExtensions.ENGINE_KEY] = this;
    return ctx;
  }

  #endregion
}