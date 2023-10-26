using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;

namespace Rubric.Engines.Implementation;

/// <summary>
///   Common base class for all engine implementations.
/// </summary>
public abstract class BaseRuleEngine : IRuleEngine
{
  /// <inheritdoc />
  public ILogger Logger { get; protected set; }

  /// <inheritdoc />
  public abstract bool IsAsync { get; }

  /// <inheritdoc />
  public abstract Type InputType { get; }

  /// <inheritdoc />
  public abstract Type OutputType { get; }

  /// <inheritdoc />
  public IExceptionHandler ExceptionHandler { get; protected set; }

  /// <summary>
  ///   Handle an otherwise unhandled exception during an engine run.
  /// </summary>
  /// <param name="ex">The exception.</param>
  /// <param name="e">The currently executing engine.</param>
  /// <param name="ctx">The context of the current engine execution.</param>
  /// <param name="input">The current input object.</param>
  /// <param name="output">The current output object.</param>
  /// <param name="rule">The rule that generated the exception.</param>
  /// <param name="t">The current cancellation token.</param>
  /// <returns>Whether the exception should be considered handled.</returns>
  protected internal static bool HandleException(
    Exception ex,
    IRuleEngine e,
    IEngineContext ctx,
    object rule,
    object input,
    object output,
    CancellationToken t = default)
  {
    if (ctx == null) throw new ArgumentNullException(nameof(ctx));
    if (e == null) throw new ArgumentNullException(nameof(e));
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
          ctx.SetLastException(ee);
          throw;
        }
      case EngineException ee:
        ee.Rule = rule;
        ee.Input = input;
        ee.Output = output;
        ee.Context = ctx;
        ctx.SetLastException(ee);
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
          ctx.SetLastException(ee);
          throw;
        }
    }
  }

  protected static void ParallelCleanup(Task t, CancellationTokenSource cts, Exception userException)
  {
    if (t == null) throw new ArgumentNullException(nameof(t));
    if (cts == null) throw new ArgumentNullException(nameof(cts));
    cts.Dispose();
    //Preserve original stack traces and throw.
    if (t.IsCanceled && userException != null)
    {
      var info = ExceptionDispatchInfo.Capture(userException);
      info.Throw();
    }
    if (t.Exception != null)
    {
      var info = ExceptionDispatchInfo.Capture(t.Exception.InnerExceptions.Last());
      info.Throw();
    }
  }
  
  internal IEngineContext SetupContext(IEngineContext ctx)
  {
    ctx ??= new EngineContext();
    ctx.SetExecutionInfo(this, Guid.NewGuid().ToString());
    ctx.ClearAllCaches();
    ctx.SetLastException(null);
    return ctx;
  }
}