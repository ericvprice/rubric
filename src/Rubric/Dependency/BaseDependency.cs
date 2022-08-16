namespace Rubric.Dependency;

public abstract class BaseDependency : IDependency
{
  /// <summary>
  ///     Read the dependencies from the DependsOn attribute(s).
  /// </summary>
  /// <returns>An enumeration of dependency names.</returns>
  public virtual IEnumerable<string> Dependencies => DependencyExtensions.GetDependencies(GetType());

  /// <summary>
  ///     Read the provided dependencies from the Provides attribute(s).
  /// </summary>
  /// <returns>The provided dependencies for this rule.</returns>
  public virtual IEnumerable<string> Provides => DependencyExtensions.GetProvides(GetType()).Append(Name);

  public abstract string Name { get; }
}
