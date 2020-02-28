using System;
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
        /// <summary>
        ///     Logger instance.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        ///     Whether this engine is async.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        ///     Whether this engine is executing rules in parallel.
        /// </summary>
        bool IsParallel { get; }

        /// <summary>
        ///     The input type for this engine.
        /// </summary>
        Type InputType { get; }

        /// <summary>
        ///     The output type for this engine.
        /// </summary>
        Type OutputType { get; }
    }

    /// <summary>
    ///     A rule engine.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public interface IRulesEngine<in TIn, in TOut> : IRulesEngine
        where TIn : class
        where TOut : class
    {
        IEnumerable<IRule<TIn>> PreRules { get; }

        IEnumerable<IRule<TIn, TOut>> Rules { get; }

        IEnumerable<IRule<TOut>> PostRules { get; }


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

    /// <summary>
    ///     A rule engine.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public interface IRulesEngine<in T> : IRulesEngine
        where T : class
    {
        IEnumerable<IRule<T>> Rules { get; }

        /// <summary>
        ///     Apply the given input to the output object.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        void Apply(T input, IEngineContext context = null);

        /// <summary>
        ///     Serially apply the given inputs to the output object.
        /// </summary>
        /// <param name="inputs">The input objects.</param>
        /// <param name="output">The output object.</param>
        /// <param name="context">An optional injected context.</param>
        void Apply(IEnumerable<T> inputs, IEngineContext context = null);

    }
}