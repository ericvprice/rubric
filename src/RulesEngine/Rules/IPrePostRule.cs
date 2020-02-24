using RulesEngine.Dependency;

namespace RulesEngine.Rules
{
    /// <summary>
    ///     Common interface for pre- and post-processing rules.
    /// </summary>
    /// <typeparam name="T">The input/output type.</typeparam>
    public interface IPrePostRule<in T> : IDependency
    {
        /// <summary>
        ///     Whether this rule should apply to the input/output.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="obj">The input or output object to evaluate.</param>
        /// <returns>Whether this rule should apply to the input/output.</returns>
        bool DoesApply(IEngineContext context, T obj);

        /// <summary>
        ///     Execute this rule on the engine input/output.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="obj">The input/output object to evaluate.</param>
        void Apply(IEngineContext context, T obj);

    }
}