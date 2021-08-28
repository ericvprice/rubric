using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine;

public class RulesEngine<TIn, TOut> : IRulesEngine<TIn, TOut>
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
  /// <param name="logger">An optional logger</param>
  public RulesEngine(Ruleset<TIn, TOut> ruleset,
                     IExceptionHandler handler = null,
                     ILogger logger = null)
      : this(ruleset.PreRules, ruleset.Rules, ruleset.PostRules, handler, logger) { }

  /// <summary>
  ///     Default public constructor.
  /// </summary>
  /// <param name="preprocessingRules">Collection of synchronous preprocessing rules.</param>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="postprocessingRules">Collection of synchronous postprocessing rules.</param>
  /// <param name="logger">An optional logger.</param>
  public RulesEngine(
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
    ExceptionHandler = exceptionHandler ?? ExceptionHandlers.Throw;
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
  public bool IsParallel => false;

  /// <inheritdoc />
  public Type InputType => typeof(TIn);

  /// <inheritdoc />
  public Type OutputType => typeof(TOut);

  public IExceptionHandler ExceptionHandler { get; }

  public EngineException LastException { get; private set; }

  #endregion

  #region Methods

  ///<inheritdoc/>
  public void Apply(TIn input, TOut output, IEngineContext context = null)
  {
    var ctx = context ?? new EngineContext();
    SetupContext(ctx);
    try
    {
      ApplyItem(input, output, ctx);

      foreach (var set in _postprocessingRules)
        foreach (var rule in set)
          ApplyPrePostRule(ctx, rule, output);
    }
    catch (EngineHaltException) { }
  }

  ///<inheritdoc/>
  public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
  {
    var ctx = context ?? new EngineContext();
    foreach (var input in inputs)
    {
      try
      {
        ApplyItem(input, output, ctx);
      } 
      catch (ItemHaltException)
      {
        continue;
      } 
      catch (EngineHaltException)
      {
        return;
      }
    }
    try
    {
      foreach (var set in _postprocessingRules)
        foreach (var rule in set)
          ApplyPrePostRule(ctx, rule, output);
    }
    catch (EngineException) { }
  }

  private void ApplyItem(TIn input, TOut output, IEngineContext ctx)
  {
      foreach (var set in _preprocessingRules)
        foreach (var rule in set)
          ApplyPrePostRule(ctx, rule, input);
      foreach (var set in _rules)
        foreach (var rule in set)
          ApplyRule(ctx, rule, input, output);
  }

  private void ApplyPrePostRule<T>(IEngineContext ctx, IRule<T> rule, T input)
  {
    try
    {
      var doesApply = rule.DoesApply(ctx, input);
      Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
      if (!doesApply) return;
      Logger.LogTrace($"Applying {rule.Name}.");
      rule.Apply(ctx, input);
      Logger.LogTrace($"Finished applying {rule.Name}.");
    }
    catch (EngineException e)
    {
      e.Rule = rule;
      e.Input = input;
      e.Context = ctx;
      LastException = e;
      throw;
    } catch (Exception ue)
    {
      bool handled;
      try
      {
        handled = ExceptionHandler.HandleException(ue, ctx, input, null, rule);
      }
      catch (ItemHaltException e)
      {
        e.Rule = rule;
        e.Input = input;
        e.Context = ctx;
        LastException = e;
        throw;
      }
      catch (EngineHaltException ehe)
      {
        ehe.Rule = rule;
        ehe.Input = input;
        ehe.Context = ctx;
        LastException = ehe;
        throw;
      }
      catch (Exception)
      {
        // The exception handler threw an exception.  Give up.
        throw;
      }
      // Throw with the user's blessing.
      if (!handled)
        throw;
    }
  }

  private void ApplyRule(IEngineContext ctx, IRule<TIn, TOut> rule, TIn input, TOut output)
  {
    try
    {
      var doesApply = rule.DoesApply(ctx, input, output);
      Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
      if (!doesApply) return;
      Logger.LogTrace($"Applying {rule.Name}.");
      rule.Apply(ctx, input, output);
      Logger.LogTrace($"Finished applying {rule.Name}.");
    }
    catch (EngineException e)
    {
      e.Rule = rule;
      e.Input = input;
      e.Context = ctx;
      e.Output = output;
      LastException = e;
      throw;
    }
    catch (Exception ue)
    {
      bool handled;
      try
      {
        handled = ExceptionHandler.HandleException(ue, ctx, input, null, rule);
      }
      catch (ItemHaltException e)
      {
        e.Rule = rule;
        e.Input = input;
        e.Context = ctx;
        e.Output = output;
        LastException = e;
        throw;
      }
      catch (EngineHaltException ehe)
      {
        ehe.Rule = rule;
        ehe.Input = input;
        ehe.Context = ctx;
        ehe.Output = output;
        LastException = ehe;
        throw;
      }
      catch (Exception)
      {
        // The exception handler threw an exception.  Give up.
        throw;
      }
      // Throw with the user's blessing.
      if (!handled)
        throw;
    }
  }

  internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;

  #endregion
}