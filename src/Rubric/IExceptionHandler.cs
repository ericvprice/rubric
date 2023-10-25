namespace Rubric;

/// <summary>
///   Define a policy for handling unhandled exceptions.
/// </summary>
public interface IExceptionHandler
{
  /// <summary>
  ///   Handle an otherwise unhandled exception during an engine run.
  /// </summary>
  /// <param name="e">The exception.</param>
  /// <param name="context">The context of the current engine execution.</param>
  /// <param name="input">The current input object.</param>
  /// <param name="output">The current output object.</param>
  /// <param name="rule">The rule that generated the exception.</param>
  /// <returns>Whether the exception should be considered handled.</returns>
  bool HandleException(Exception e, IEngineContext context, object input, object output, object rule);
}