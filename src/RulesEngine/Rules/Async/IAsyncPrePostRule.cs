using System.Threading.Tasks;
using RulesEngine.Dependency;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     Common interface for asynchronous pre- and post-processing rules.
    /// </summary>
    /// <typeparam name="T">The input or output type.</typeparam>
    public interface IAsyncPrePostRule<in T> : IDependency
    {
        /// <summary>
        ///     Asynchronously determine whether this rule should apply.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="obj">The input or output object to evaluate.</param>
        /// <returns>Whether this rule should apply to the input/output.</returns>
        Task<bool> DoesApply(IEngineContext context, T obj);

        /// <summary>
        ///     Execute this rule asynchronously on the engine input/output.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="obj">The input or output object to evaluate.</param>
        Task Apply(IEngineContext context, T obj);

    }
}