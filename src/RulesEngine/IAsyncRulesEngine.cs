using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RulesEngine.Rules.Async;

namespace RulesEngine
{

    public interface IAsyncRulesEngine<in TIn, in TOut>
    {

        /// <summary>
        ///     The preprocessing rules for this engine.
        /// </summary>
        IEnumerable<IAsyncPreRule<TIn>> PreRules { get; }

        /// <summary>
        ///     The rules for this engine.
        /// </summary>
        IEnumerable<IAsyncRule<TIn, TOut>> Rules { get; }

        /// <summary>
        ///     The postprocessing rules for this engine.
        /// </summary>
        IEnumerable<IAsyncPostRule<TOut>> PostRules { get; }

        bool ProcessInParallel { get; set; }

        ILogger Logger { get; }

        /// <summary>
        ///     Apply the given input to the output object.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        Task ApplyAsync(TIn input, TOut output);

        /// <summary>
        ///     Serially apply the given inputs to the output object.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="output">The output object.</param>
        Task ApplyAsync(IEnumerable<TIn> inputs, TOut output);

    }

}