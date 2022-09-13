namespace Rubric;

/// <summary>
///  Base engine exception class.
/// </summary>
[Serializable]
public class EngineException : Exception
{

  public EngineException(string message, Exception innerException) : base(message, innerException)
  {
  }

  /// <summary>
  ///   The current engine context.
  /// </summary>
  /// <value>The current engine context.</value>
  public IEngineContext Context { get; internal set; }

  /// <summary>
  ///   The current input object being processed, if any.
  /// </summary>
  /// <value>The input object.</value>
  public object Input { get; internal set; }

  /// <summary>
  ///   The current output object being processed, if any.
  /// </summary>
  /// <value>The output object.</value>
  public object Output { get; internal set; }

  /// <summary>
  ///   The current rule being executed.
  /// </summary>
  /// <value>The rule.</value>
  public object Rule { get; internal set; }

}
