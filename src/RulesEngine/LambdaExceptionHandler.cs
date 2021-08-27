namespace RulesEngine;

public class LambdaExceptionHandler : IExceptionHandler
{
    private readonly Action<Exception, IEngineContext, object, object, object> _handler;

    public LambdaExceptionHandler(Action<Exception, IEngineContext, object, object, object> handler) 
        => _handler = handler ?? throw new ArgumentException(nameof(handler));

    public void HandleException(Exception e, IEngineContext context, object input, object output, object rule)
        => _handler(e, context, input, output, rule);
}