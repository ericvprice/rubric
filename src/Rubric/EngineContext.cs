using System.Collections.Concurrent;

namespace Rubric;

/// <inheritdoc />
public class EngineContext : IEngineContext
{
  private readonly ConcurrentDictionary<string, object> _stash = new();

  /// <inheritdoc />
  public object this[string key]
  {
    get => _stash[key];
    set => _stash[key] = value;
  }

  /// <inheritdoc />
  public bool ContainsKey(string key) => _stash.ContainsKey(key);

  /// <inheritdoc />
  public T GetAs<T>(string key) => (T)_stash[key];

  /// <inheritdoc />
  public T GetOrSet<T>(string key, Func<T> factory)
  {
    if (factory == null) throw new ArgumentNullException(nameof(factory)); 
    return (T)_stash.GetOrAdd(key, factory());
  }

  /// <inheritdoc />
  public void Remove(string key) => _stash.Remove(key, out _);

  /// <inheritdoc />
  public EngineContext Clone()
  {
    var toReturn = new EngineContext();
    foreach (var name in _stash.Keys)
      toReturn._stash[name] = _stash[name];
    return toReturn;
  }
}