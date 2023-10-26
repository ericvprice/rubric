using System.Collections;
using System.Collections.Concurrent;

namespace Rubric;

/// <summary>
///   Concurrent dictionary with thread-safe async GetOrAdd method.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public class AsyncConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{

  private readonly ConcurrentDictionary<TKey, TValue> _inner = new();
  private readonly ConcurrentDictionary<TKey, Lazy<SemaphoreSlim>> _semaphoreSlimDictionary = new();

  private SemaphoreSlim GetSemaphoreSlim(TKey key) 
    => _semaphoreSlimDictionary.GetOrAdd(key, _ => new(() => new(1, 1))).Value;

  /// <summary>
  ///   Get or add an entry with a deferred async factory method.
  /// </summary>
  /// <param name="key">The key.</param>
  /// <param name="factory">The factory function.</param>
  /// <returns>The retrieved or new value.</returns>
  public async Task<TValue> GetOrAddAsync(TKey key, Func<TKey, Task<TValue>> factory)
  {
    if(factory == null) throw new ArgumentNullException(nameof(factory));
    if (_inner.TryGetValue(key, out var value))
      return value;
    var semaphoreSlim = GetSemaphoreSlim(key);
    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
    try
    {
      // double check after lock
      if (_inner.TryGetValue(key, out value))
        return value;
      value = await factory(key).ConfigureAwait(false);
      _inner.TryAdd(key, value);
    }
    finally
    {
      semaphoreSlim.Release();
    }
    return value;
  }

  /// <inheritdoc />
  public void Add(KeyValuePair<TKey, TValue> item) => _inner[item.Key] = item.Value;

  /// <summary>
  ///   Clear the contents of this dictionary.
  /// </summary>
  public void Clear() => _inner.Clear();

  /// <inheritdoc />
  public bool Contains(KeyValuePair<TKey, TValue> item) => _inner.Contains(item);

  /// <inheritdoc />
  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).CopyTo(array, arrayIndex);

  /// <inheritdoc />
  public bool Remove(KeyValuePair<TKey, TValue> item) => _inner.Remove(item.Key, out _);

  /// <inheritdoc />
  public int Count => _inner.Count;

  /// <inheritdoc />
  public bool IsReadOnly => false;

  /// <summary>
  ///   Get or add an entry with a deferred factory method.
  /// </summary>
  /// <param name="key">The key.</param>
  /// <param name="func">The function.</param>
  /// <returns></returns>
  public TValue GetOrAdd(TKey key, Func<TKey, TValue> func)
  {
    if (func == null) throw new ArgumentNullException(nameof(func));
    return _inner.GetOrAdd(key, func(key));
  }

  /// <inheritdoc />
  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _inner.GetEnumerator();

  /// <inheritdoc />
  IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();

  /// <inheritdoc />
  public void Add(TKey key, TValue value) => _inner[key] = value;

  /// <inheritdoc />
  public bool ContainsKey(TKey key) => _inner.ContainsKey(key);

  /// <inheritdoc />
  public bool Remove(TKey key) => _inner.Remove(key, out _);

  /// <inheritdoc />
  public bool TryGetValue(TKey key, out TValue value) => _inner.TryGetValue(key, out value);

  /// <inheritdoc />
  public TValue this[TKey key]
  {
    get => _inner[key];
    set => _inner[key] = value;
  }

  /// <inheritdoc />
  public ICollection<TKey> Keys => _inner.Keys;

  /// <inheritdoc />
  public ICollection<TValue> Values => _inner.Values;
}