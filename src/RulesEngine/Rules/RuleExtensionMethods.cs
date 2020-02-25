using RulesEngine.Rules.Async;

namespace RulesEngine.Rules
{
    public static class RuleExtensionMethods
    {
        /// <summary>
        ///     Wrap a synchronous input rule in an asynchronous wrapper.
        /// </summary>
        /// <param name="syncRule">The synchronous rule.</param>
        /// <typeparam name="TIn">The engine input type.</typeparam>
        /// <returns>An async wrapper for this rule.</returns>
        public static IAsyncPreRule<TIn> WrapAsync<TIn>(this IPreRule<TIn> syncRule) =>
            new AsyncPreRuleWrapper<TIn>(syncRule);

        /// <summary>
        ///     Wrap a synchronous output rule in an asynchronous wrapper.
        /// </summary>
        /// <param name="syncRule">The synchronous rule.</param>
        /// <typeparam name="TOut">The engine output type.</typeparam>
        /// <returns>An async wrapper for this rule.</returns>
        public static IAsyncPostRule<TOut> WrapAsync<TOut>(this IPostRule<TOut> syncRule) =>
            new AsyncPostRuleWrapper<TOut>(syncRule);

        /// <summary>
        ///     Wrap an synchronous rule in an asynchronous wrapper.
        /// </summary>
        /// <param name="syncRule">The rule to write.</param>
        /// <typeparam name="TIn">The input type.</typeparam>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <returns>An async wrapper for this rule.</returns>
        public static IAsyncRule<TIn, TOut> WrapAsync<TIn, TOut>(this IRule<TIn, TOut> syncRule) =>
            new AsyncRuleWrapper<TIn, TOut>(syncRule);
    }
}