namespace Rubric.Dependency;

/// <summary>
///   An object that can participate in a dependency graph.
/// </summary>
public interface IDependency
{
  /// <summary>
  ///   GetAs the dependencies for this dependency.
  /// </summary>
  IEnumerable<string> Dependencies { get; }

  /// <summary>
  ///   GetAs the dependencies this provides to other dependencies.
  /// </summary>
  IEnumerable<string> Provides { get; }

  /// <summary>
  ///   The name of this dependency.  <see cref="Provides">Provides</see>always contains at least this name.
  /// </summary>
  string Name { get; }
}