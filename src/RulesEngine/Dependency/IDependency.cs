namespace RulesEngine.Dependency
{
    public interface IDependency
    {
        /// <summary>
        ///     Get the dependencies for this dependency.
        /// </summary>
        IEnumerable<string> Dependencies { get; }

        /// <summary>
        ///     Get the dependencies this provides to other dependencies.
        /// </summary>
        IEnumerable<string> Provides { get; }

        /// <summary>
        ///     The name of this dependency.  <see cref="Provides">Provides</see>always contains at least this name.
        /// </summary>
        string Name { get; }
    }
}