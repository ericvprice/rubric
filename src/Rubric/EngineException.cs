using System.Runtime.Serialization;

namespace Rubric;

/// <summary>
///   Base engine exception class.
/// </summary>
[Serializable]
public class EngineException : Exception
{
  /// <summary>
  ///   Default constructor
  /// </summary>
  public EngineException() { }

  /// <summary>
  ///   Message constructor.
  /// </summary>
  public EngineException(string message) : base(message) { }

  /// <summary>
  ///   Message and exception constructor.
  /// </summary>
  /// <param name="message">The exception message.</param>
  /// <param name="innerException">The original exception thrown.</param>
  public EngineException(string message, Exception innerException) : base(message, innerException) { }

  /// <summary>
  ///   Serialization constructor.
  /// </summary>
  /// <param name="serializationInfo">
  ///   The SerializationInfo that holds the serialized object data about the exception being
  ///   thrown.
  /// </param>
  /// <param name="streamingContext">
  ///   The StreamingContext that contains contextual information about the source or
  ///   destination.
  /// </param>
  protected EngineException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
    serializationInfo, streamingContext) { }

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