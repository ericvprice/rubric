namespace RulesEngine;

/// <summary>
///     Define a policy for handling unhandled exceptions.
/// </summary>
public interface IExceptionHandler
{

  void HandleException(Exception e, IEngineContext context, object Input, object output, object rule);

}