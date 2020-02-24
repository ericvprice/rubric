using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RulesEngine.Rules;

namespace RulesEngine
{
    public interface IRulesEngine<in TIn, in TOut>
    {

        IEnumerable<IPreRule<TIn>> PreRules { get; }

        IEnumerable<IRule<TIn, TOut>> Rules { get; }

        IEnumerable<IPostRule<TOut>> PostRules { get; }

        ILogger Logger { get; }

        /// <summary>
        ///     Apply the given input to the output object.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        void Apply(TIn input, TOut output);

        /// <summary>
        ///     Serially apply the given inputs to the output object.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="output">The output object.</param>
        void Apply(IEnumerable<TIn> inputs, TOut output);

    }

}