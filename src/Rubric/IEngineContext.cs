namespace Rubric;

public interface IEngineContext
{
  /// <summary>
  ///     Get/set arbitrary values by name
  /// </summary>
  object this[string name] { get; set; }

  /// <summary>
  ///     Check if an arbitrary value exists by name;
  /// </summary>
  bool ContainsKey(string name);

  /// <summary>
  ///     Remove an object by name.
  /// </summary>
  void Remove(string name);

  /// <summary>
  ///     Get arbitrary value by name as T.
  /// </summary>
  /// <param name="name">The name of the value.</param>
  /// <typeparam name="T">The type to cast to.</typeparam>
  T Get<T>(string name);

  EngineContext Clone();
}
