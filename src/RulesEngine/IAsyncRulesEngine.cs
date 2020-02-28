using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;

namespace RulesEngine
{
    public interface IAsyncRulesEngine<in TIn, in TOut> : IRulesEngine
        where TIn : class
        where TOut : class
    {
        /// <summary>
        ///     The preprocessing rules for this engine.
        /// </summary>
        IEnumerable<IAsyncRule<TIn>> PreRules { get; }

        /// <summary>
        ///     The rules for this engine.
        /// </summary>
        IEnumerable<IAsyncRule<TIn, TOut>> Rules { get; }

        /// <summary>
        ///     The postprocessing rules for this engine.
        /// </summary>
        IEnumerable<IAsyncRule<TOut>> PostRules { get; }

        /// <summary>
        ///     Apply the given input to the output object.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        Task ApplyAsync(TIn input, TOut output, IEngineContext context = null);

        /// <summary>
        ///     Serially apply the given inputs to the output object.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null);
    }

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
        Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null);
    }
}