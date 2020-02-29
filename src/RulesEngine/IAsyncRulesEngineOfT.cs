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
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        Task ApplyAsync(T input, IEngineContext context = null);

        /// <summary>
        ///     Serially apply the given inputs to the output object.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        /// <param name="parallelizeInputs">Whether to parallelize on inputs.<param>
        Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null, bool parallelizeInputs = false);
    }
}