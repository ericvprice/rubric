using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RulesEngine.Rules;

namespace RulesEngine
{
    /// <summary>
    ///     Basic interface for rule engines.
    /// </summary>
    public interface IRulesEngine
    {
        ILogger Logger { get; }

        bool IsAsync { get; }

        bool IsParallel { get; }
    }

    /// <summary>
    ///     A rule engine.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public interface IRulesEngine<in TIn, in TOut> : IRulesEngine
    {
        IEnumerable<IPreRule<TIn>> PreRules { get; }

        IEnumerable<IRule<TIn, TOut>> Rules { get; }

        IEnumerable<IPostRule<TOut>> PostRules { get; }


        /// <summary>
        ///     Apply the given input to the output object.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        void Apply(TIn input, TOut output, IEngineContext context = null);

        /// <summary>
        ///     Serially apply the given inputs to the output object.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null);
    }
}