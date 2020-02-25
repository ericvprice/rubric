using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     Async wrapper that proxies a pre- or post- processing rule.
    /// </summary>
    /// <typeparam name="T">The input/output type.</typeparam>
    public class AsyncPrePostRuleWrapper<T> : IAsyncPrePostRule<T>
    {
        private readonly IPrePostRule<T> _syncRule;

        /// <summary>
        ///     Create a wrapper around the equivalent synchronous rule.
        /// </summary>
        /// <param name="syncRule">The synchronous rule.</param>
        public AsyncPrePostRuleWrapper(IPrePostRule<T> syncRule)
            => _syncRule = syncRule;

        /// <inheritdoc />
        public Task Apply(IEngineContext context, T input)
        {
            _syncRule.Apply(context, input);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> DoesApply(IEngineContext context, T input)
            => Task.FromResult(_syncRule.DoesApply(context, input));

        /// <inheritdoc />
        public string Name => _syncRule.Name + " (wrapped async)";

        /// <inheritdoc />
        public IEnumerable<string> Dependencies => _syncRule.Dependencies;

        /// <inheritdoc />
        public IEnumerable<string> Provides => _syncRule.Provides;
    }
}