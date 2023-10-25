using System.Runtime.Serialization;

namespace Rubric;

/// <summary>
///     A flow control exception that can be thrown to influence the execution of the engine.
///     Throwing this exception will:
///     1) Halt all further synchronous calls on the current item being processed
///     2) Cancel all further asynchronous calls on the current item (behavior determined by currently executing rules)
///     3) If processing multiple items, further items will be processed.
///     4) For asynchronous engines, the engine will exit with a TaskCancelledException (with )
/// </summary>
[Serializable]
public class ItemHaltException : EngineException
{
  /// <summary>
  ///   Default constructor.
  /// </summary>
  public ItemHaltException() { }

  /// <summary>
  ///  Message constructor.
  /// </summary>
  public ItemHaltException(string message) : base(message) { }

  /// <summary>
  ///   Constructor with custom message and optional inner exception.
  /// </summary>
  public ItemHaltException(string message, Exception innerException = null) : base(message, innerException) { }

  /// <summary>
  ///   Serialization constructor.
  /// </summary>
  /// <param name="serializationInfo">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
  /// <param name="streamingContext">The StreamingContext that contains contextual information about the source or destination.</param>
  protected ItemHaltException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
  {
  }
}