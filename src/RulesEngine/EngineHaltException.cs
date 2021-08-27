namespace RulesEngine;

/// <summary>
///     A flow control statement that can be thrown to influence the execution of the engine.
///     Throwing this exception will:
///     1) Halt all further synchronous calls on the current item being processed
///     2) Cancel all further asychronous calls on the current item (behavior determined by currently executing rules)
///     3) If processing multiple items, no further items will be processed.
///     4) For asynchronous engines, the engine will exit with a TaskCancelledException.
/// </summary>
[Serializable]
public class EngineHaltException : EngineException
{
  public EngineHaltException() : this("User requested", null) { }

  public EngineHaltException(string message, Exception innerException) : base(message, innerException) { }
}