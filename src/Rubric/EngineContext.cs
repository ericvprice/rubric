using System.Collections.Concurrent;

namespace Rubric;

public class EngineContext : IEngineContext
{
  private readonly ConcurrentDictionary<string, object> _stash = new();

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

  public T GetOrSet<T>(string name, Func<T> factory) => (T) _stash.GetOrAdd(name, factory());

  /// <inheritdoc />
  public void Remove(string name) => _stash.Remove(name, out _);

  /// <inheritdoc />
  public EngineContext Clone()
  {
    var toReturn = new EngineContext();
    foreach (var name in _stash.Keys)
      toReturn._stash[name] = _stash[name];
    return toReturn;
  }
}