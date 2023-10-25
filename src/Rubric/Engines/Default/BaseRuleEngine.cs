using Microsoft.Extensions.Logging;

namespace Rubric.Engines.Default;

public abstract class BaseRuleEngine : IRuleEngine
{
  public ILogger Logger { get; protected set; }

  public abstract bool IsAsync { get; }

  public abstract Type InputType { get; }

  public abstract Type OutputType { get; }

  public IExceptionHandler ExceptionHandler { get; protected set; }

  protected internal static bool HandleException(Exception ex, IRuleEngine e, IEngineContext ctx, object rule,
                                                 object input, object output, CancellationToken t = default)
  {
    switch (ex)
    {
      //Ignore user-requested task cancellation exceptions
      case TaskCanceledException tce:
        if (t == tce.CancellationToken) return false;
        try
        {
          return e.ExceptionHandler.HandleException(ex, ctx, input, null, rule);
        }
        catch (EngineException ee)
        {
          ee.Rule = rule;
          ee.Input = input;
          ee.Output = output;
          ee.Context = ctx;
          ctx[EngineContextExtensions.LAST_EXCEPTION_KEY] = ee;
          throw;
        }
      case EngineException ee:
        ee.Rule = rule;
        ee.Input = input;
        ee.Output = output;
        ee.Context = ctx;
        ctx[EngineContextExtensions.LAST_EXCEPTION_KEY] = ee;
        return false;
      default:
        try
        {
          return e.ExceptionHandler.HandleException(ex, ctx, input, null, rule);
        }
        catch (EngineException ee)
        {
          ee.Rule = rule;
          ee.Input = input;
          ee.Output = output;
          ee.Context = ctx;
          ctx[EngineContextExtensions.LAST_EXCEPTION_KEY] = ee;
          throw;
        }
    }
  }

  internal IEngineContext SetupContext(IEngineContext ctx)
  {
    ctx ??= new EngineContext();
    ctx[EngineContextExtensions.ENGINE_KEY] = this;
    ctx[EngineContextExtensions.TRACE_ID_KEY] = Guid.NewGuid().ToString();
    ctx.GetExecutionPredicateCache().Clear();
    ctx.GetItemPredicateCache().Clear();
    return ctx;
  }
}