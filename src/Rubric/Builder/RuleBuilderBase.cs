namespace Rubric.Builder;

/// <summary>
///   Common properties and methods shared by all rule builders.
/// </summary>
internal abstract class RuleBuilderBase
{

  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="name">The name for this rule.</param>
  /// <exception cref="ArgumentException">Name is null or empty.</exception>
  protected internal RuleBuilderBase(string name)
  {
    Name = string.IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
    Provides.Add(Name);
  }

  /// <summary>
  ///   The list of dependencies for this rule.
  /// </summary>
  internal List<string> Dependencies { get; } = new();

  /// <summary>
  ///   The list of dependencies provided by this rule.
  /// </summary>
  internal List<string> Provides { get; } = new();

  /// <summary>
  ///   The predicate caching behavior of this engine.
  /// </summary>
  internal PredicateCaching Caching { get; set; }

  /// <summary>
  ///   The name of this rule.
  /// </summary>
  internal string Name { get; }

  /// <summary>
  ///   Add a named dependency for this rule.
  /// </summary>
  /// <param name="dependency">The dependency.</param>
  /// <exception cref="ArgumentException">The string is null or empty.</exception>
  internal void AddDependency(string dependency)
  {
    if (string.IsNullOrEmpty(dependency)) throw new ArgumentException("dependency cannot be null or empty", nameof(dependency));
    Dependencies.Add(dependency);
  }

  /// <summary>
  ///   Add a dependency for this rule by type.
  /// </summary>
  /// <param name="dependency">The dependency.</param>
  /// <exception cref="ArgumentException">The string is null or empty.</exception>
  internal void AddDependency(Type dependency)
    => Dependencies.Add(dependency?.FullName ?? throw new ArgumentNullException(nameof(dependency)));

  /// <summary>
  ///   Add a dependency that this rule provides.
  /// </summary>
  /// <param name="provides">The dependency provided.</param>
  /// <exception cref="ArgumentException">The name is null or empty.</exception>
  internal void AddProvides(string provides)
  {
    if (string.IsNullOrWhiteSpace(provides))
      throw new ArgumentException("Provides cannot be null or empty", nameof(provides));
    Provides.Add(provides);
  }
}