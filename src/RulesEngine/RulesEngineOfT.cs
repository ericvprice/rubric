using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine;

public class RulesEngine<T> : IRulesEngine<T>
    where T : class
{

  #region Fields

  private readonly IRule<T>[][] _rules;

  #endregion

  #region Constructors

  public RulesEngine(Ruleset<T> ruleset,
                      IExceptionHandler uncaughtExceptionHandler = null,
                      ILogger logger = null)
      : this(ruleset.Rules, uncaughtExceptionHandler, logger) { }

  /// <summary>
  ///     Default public constructor.
  /// </summary>
  /// <param name="rules">Collection of synchronous processing rules.</param>
  /// <param name="logger">An optional logger.</param>
  public RulesEngine(
      IEnumerable<IRule<T>> rules,
      IExceptionHandler uncaughtExceptionHandler = null,
      ILogger logger = null
  )
  {
    rules ??= Enumerable.Empty<IRule<T>>();
    _rules = rules.ResolveDependencies()
                    .Select(e => e.ToArray())
                    .ToArray();
    Logger = logger ?? NullLogger.Instance;
    ExceptionHandler = uncaughtExceptionHandler ?? ExceptionHandlers.Throw;
  }

  #endregion

  #region Properties

  public IEnumerable<IRule<T>> Rules
      => _rules.SelectMany(_ => _);

  /// <inheritdoc />
  public ILogger Logger { get; }

  /// <inheritdoc />
  public bool IsAsync => false;

  /// <inheritdoc />
  public bool IsParallel => false;

  /// <inheritdoc />
  public Type InputType => typeof(T);

  /// <inheritdoc />
  public Type OutputType => typeof(T);

  /// <inheritdoc />
  public IExceptionHandler ExceptionHandler { get; }

  public EngineException LastException { get; set; }

  #endregion

  #region Methods

  ///<inheritdoc/>
  public void Apply(T input, IEngineContext context = null)
  {
    var ctx = Reset(context);
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
  
  private IEngineContext Reset(IEngineContext context)
  {
    context ??= new EngineContext();
    SetupContext(context);
    LastException = null;
    return context;
  }

  internal IEngineContext SetupContext(IEngineContext ctx)
  {
    ctx ??= new EngineContext();
    ctx[EngineContextExtensions.ENGINE_KEY] = this;
    return ctx;
  }

  #endregion

}