namespace RulesEngine;

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
  public ItemHaltException() : this("User requested.", null) { }

  public ItemHaltException(string message, Exception innerException) : base(message, innerException) { }
}