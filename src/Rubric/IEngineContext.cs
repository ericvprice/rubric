namespace Rubric;

/// <summary>
///   A context for engine execution.
/// </summary>
public interface IEngineContext
{
  /// <summary>
  ///   GetAs/set arbitrary values by key
  /// </summary>
  object this[string key] { get; set; }

  /// <summary>
  ///   Check if an arbitrary value exists by key;
  /// </summary>
  bool ContainsKey(string key);

  /// <summary>
  ///   Remove an object by key.
  /// </summary>
  void Remove(string key);

  /// <summary>
  ///   Get arbitrary value by key as T.
  /// </summary>
  /// <param name="key">The key of the value.</param>
  /// <typeparam name="T">The type to cast to.</typeparam>
  /// <returns>The value.</returns>
  T GetAs<T>(string key);

  /// <summary>
  ///   Clone this engine context, making a shallow copy of all the stash entries.
  /// </summary>
  /// <returns>A copy of the engine context.</returns>
  EngineContext Clone();

  /// <summary>
  ///   GetAs an item, or add a default item if one is not present.
  /// </summary>
  /// <typeparam name="T">The type of the object to retrieve.</typeparam>
  /// <param name="key">The key of the object.</param>
  /// <param name="factory">Factory function to generate the default item.</param>
  /// <returns>The value.</returns>
  T GetOrSet<T>(string key, Func<T> factory);
}