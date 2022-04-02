namespace Rubric;

/// <summary>
///     Define a policy for handling unhandled exceptions.
/// </summary>
public interface IExceptionHandler
{

  bool HandleException(Exception e, IEngineContext context, object input, object output, object rule);

}