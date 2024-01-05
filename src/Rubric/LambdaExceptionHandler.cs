namespace Rubric;

internal class LambdaExceptionHandler(Func<Exception, IEngineContext, object, object, object, bool> handler)
  : IExceptionHandler
{
  private readonly Func<Exception, IEngineContext, object, object, object, bool> _handler = handler ?? throw new ArgumentNullException(nameof(handler));

  public bool HandleException(Exception e, IEngineContext context, object input, object output, object rule)
    => _handler(e, context, input, output, rule);
}