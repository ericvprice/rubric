namespace Rubric;

public class EngineContext : IEngineContext
{
  private readonly Dictionary<string, object> _stash = new();

  /// <inheritdoc />
  public object this[string name]
  {
    get => _stash[name];
    set => _stash[name] = value;
  }

  /// <inheritdoc />
  public bool ContainsKey(string name) => _stash.ContainsKey(name);

  /// <inheritdoc />
  public T Get<T>(string name) => (T)_stash[name];

  /// <inheritdoc />
  public void Remove(string name) => _stash.Remove(name);

  /// <inheritdoc />
  public EngineContext Clone()
  {
    var toReturn = new EngineContext();
    foreach (var name in _stash.Keys)
      toReturn._stash[name] = _stash[name];
    return toReturn;
  }
}