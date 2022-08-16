using Microsoft.Extensions.Logging;

namespace Rubric.Engines;

public abstract class BaseRuleEngine : IRuleEngine
{
  public ILogger Logger { get; protected set; } 

  public abstract bool IsAsync { get; }

  public abstract Type InputType { get; } 

  public abstract Type OutputType { get; }

  public IExceptionHandler ExceptionHandler { get; protected set; }

  public EngineException LastException { get; protected set; }

  protected internal bool HandleException(Exception ex, IRuleEngine e, IEngineContext ctx, object rule, object input, object output, CancellationToken t = default)
  {
    switch (ex)
    {
      //Ignore user-requested task cancellation exceptions
      case TaskCanceledException tce:
        if (t == tce.CancellationToken)
        {
          return false;
        }
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
          LastException = ee;
          throw;
        }
      case EngineException ee:
        ee.Rule = rule;
        ee.Input = input;
        ee.Output = output;
        ee.Context = ctx;
        LastException = ee;
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
          LastException = ee;
          throw;
        }
    }
  }
}
