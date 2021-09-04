namespace Rubric;

/// <summary>
///  Base exception class.
/// </summary>
[Serializable]
public class EngineException : Exception
{

  public EngineException(string message, Exception innerException) : base(message, innerException)
  {
  }

  public IEngineContext Context { get; internal set; }

  public object Input { get; internal set; }

  public object Output { get; internal set; }

  public object Rule { get; internal set; }

}
