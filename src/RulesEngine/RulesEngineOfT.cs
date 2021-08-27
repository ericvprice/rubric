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
    ExceptionHandler = uncaughtExceptionHandler;
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
    foreach (var set in _rules)
      foreach (var rule in set)
        try
        {
          ApplyRule(ctx, rule, input);
          //Expected exceptions.  Just halt.
        }
        catch (EngineException e)
        {
          e.Rule = rule;
          e.Input = input;
          e.Context = ctx;
          LastException = e;
          break;
          //Unexpected exceptions.  Invoke handler.
        }
        catch (Exception ue)
        {
          try
          {
            ExceptionHandler.HandleException(ue, ctx, input, null, rule);
          }
          catch (EngineException e)
          {
            e.Rule = rule;
            e.Input = input;
            e.Context = ctx;
            LastException = e;
            break;
          }
          catch (Exception)
          {
            //We gave them a chance...
            throw;
          }
          throw;
        }
  }

  ///<inheritdoc/>
  public void Apply(IEnumerable<T> inputs, IEngineContext context = null)
  {
    var ctx = Reset(context);
    foreach (var input in inputs)
    {
      foreach (var set in _rules)
        foreach (var rule in set)
          try
          {
            ApplyRule(ctx, rule, input);
            //Expected exceptions.  Just halt.
          }
          catch (EngineHaltException ehe)
          {
            ehe.Rule = rule;
            ehe.Input = input;
            ehe.Context = ctx;
            LastException = ehe;
            goto END;
          }
          catch (ItemHaltException ihe)
          {
            ihe.Rule = rule;
            ihe.Input = input;
            ihe.Context = ctx;
            LastException = ihe;
            goto NEXT_ITEM;
            //Unexpected exceptions.  Invoke handler.
          }
          catch (Exception ue)
          {
            try
            {
              ExceptionHandler.HandleException(ue, ctx, input, null, rule);
            }
            catch (EngineHaltException ehe)
            {
              ehe.Rule = rule;
              ehe.Input = input;
              ehe.Context = ctx;
              LastException = ehe;
              goto END;
            }
            catch (ItemHaltException ihe)
            {
              ihe.Rule = rule;
              ihe.Input = input;
              ihe.Context = ctx;
              LastException = ihe;
              //Yeah, yeah, I know.  But this is really the cleanest option.
              goto NEXT_ITEM;
            }
            catch (Exception)
            {
              throw;
            }
          }
        NEXT_ITEM:;
    }
  END:;
  }

  /// <summary>
  ///     Apply a rule.  Handle trace logging.
  /// </summary>
  /// <param name="context">Engine context.</param>
  /// <param name="rule">The current rule.</param>
  /// <param name="input">The current input item.</param>
  private void ApplyRule(IEngineContext context, IRule<T> rule, T input)
  {
    var doesApply = rule.DoesApply(context, input);
    Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
    if (!doesApply) return;
    Logger.LogTrace($"Applying {rule.Name}.");
    rule.Apply(context, input);
    Logger.LogTrace($"Finished applying {rule.Name}.");
  }
  private IEngineContext Reset(IEngineContext context)
  {
    context ??= new EngineContext();
    SetupContext(context);
    LastException = null;
    return context;
  }

  internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;

  #endregion

}