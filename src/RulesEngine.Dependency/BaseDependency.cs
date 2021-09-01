namespace RulesEngine.Dependency
{
  public abstract class BaseDependency : IDependency
  {
    /// <summary>
    ///     Read the dependencies from the DependsOn attribute(s).
    /// </summary>
    /// <returns>An enumeration of dependency names.</returns>
    public virtual IEnumerable<string> Dependencies =>
        GetType()
            .GetCustomAttributes(true)
            .OfType<DependsOnAttribute>()
            .Select(d => d.Name);

    /// <summary>
    ///     Read the provided dependencies from the Provides attribute(s).
    /// </summary>
    /// <returns>The provided dependencies for this rule.</returns>
    public virtual IEnumerable<string> Provides =>
        GetType()
            .GetCustomAttributes(true)
            .OfType<ProvidesAttribute>()
            .Select(d => d.Name)
            .Append(Name);

    public abstract string Name { get; }
  }
}