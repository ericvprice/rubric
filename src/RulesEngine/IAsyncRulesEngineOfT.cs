using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;

namespace RulesEngine
{
    public interface IAsyncRulesEngine<in T> : IRulesEngine
        where T : class
    {
        /// <summary>
        ///     The rules for this engine.
        /// </summary>
        IEnumerable<IAsyncRule<T>> Rules { get; }

        /// <summary>
        ///     Apply the given input to the output object.
        /// </summary>
        /// <param name="input">The input object.</param>
        Task ApplyAsync(T input);

        /// <summary>
        ///     Apply rules to the input object.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <param name="context">An injected context.</param>
        Task ApplyAsync(T input, IEngineContext context);

        /// <summary>
        ///     Apply rules to the input objects.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="parallelizeInputs">Whether to parallelize execution on inputs.<param>
        Task ApplyAsync(IEnumerable<T> inputs, bool parallelizeInputs = false);

        /// <summary>
        ///     Apply rules to the input objects.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="context">An injected context.</param>
        /// <param name="parallelizeInputs">Whether to parallelize execution on inputs.<param>
        Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context, bool parallelizeInputs = false);
    }
}