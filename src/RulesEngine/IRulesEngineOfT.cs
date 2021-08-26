using RulesEngine.Rules;

namespace RulesEngine
{

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