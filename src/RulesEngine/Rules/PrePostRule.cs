using RulesEngine.Dependency;

namespace RulesEngine.Rules
{

    /// <summary>
    ///     Abstract rule suitable for extension using attributes
    ///     for declarative dependencies.
    /// </summary>
    /// <typeparam name="T">The input or output type.</typeparam>
    public abstract class PrePostRule<T> : BaseDependency, IPrePostRule<T>
    {

        /// <summary>
        ///     The name for this rule, by default the name of it's type.
        /// </summary>
        /// <value></value>
        public override string Name => GetType().FullName;

        /// <inheritdoc />
        public abstract void Apply(IEngineContext context, T obj);

        /// <inheritdoc />
        public abstract bool DoesApply(IEngineContext context, T obj);

    }
}