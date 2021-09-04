namespace Rubric;

public class LambdaExceptionHandler : IExceptionHandler
{
  private readonly Func<Exception, IEngineContext, object, object, object, bool> _handler;

  public LambdaExceptionHandler(Func<Exception, IEngineContext, object, object, object, bool> handler)
      => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

  public bool HandleException(Exception e, IEngineContext context, object input, object output, object rule)
      => _handler(e, context, input, output, rule);
}