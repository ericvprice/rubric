using System.Threading;
using System.Threading.Tasks;
using RulesEngine.Dependency;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     An asynchronous engine processing rule.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public interface IAsyncRule<in T> : IDependency
    {
        /// <summary>
        ///     Whether this rule should apply to the given input, output, and
        ///     execution context.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        /// <returns>An awaitable task returning whether this rule should apply.</returns>
        Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token);

        /// <summary>
        ///     Apply this rule on the given input and output objects.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        /// <returns>An awaitable task.</returns>
        Task Apply(IEngineContext context, T input, CancellationToken token);
    }
}