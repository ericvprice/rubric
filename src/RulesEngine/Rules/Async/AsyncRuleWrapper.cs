﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     Asynchronous wrapper for a synchronous rule.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public class AsyncRuleWrapper<TIn, TOut> : IAsyncRule<TIn, TOut>
    {
        private readonly IRule<TIn, TOut> _syncRule;

        /// <summary>
        ///     Create a wrapper around the equivalent synchronous rule.
        /// </summary>
        /// <param name="syncRule">The synchronous rule.</param>
        public AsyncRuleWrapper(IRule<TIn, TOut> syncRule) => _syncRule = syncRule;

        /// <inheritdoc />
        public Task Apply(IEngineContext context, TIn input, TOut output)
        {
            _syncRule.Apply(context, input, output);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> DoesApply(IEngineContext context, TIn input, TOut output)
            => Task.FromResult(_syncRule.DoesApply(context, input, output));

        /// <inheritdoc />
        public string Name => _syncRule.Name + " (wrapped async)";

        /// <inheritdoc />
        public IEnumerable<string> Dependencies => _syncRule.Dependencies;

        /// <inheritdoc />
        public IEnumerable<string> Provides => _syncRule.Provides;
    }

    /// <summary>
    ///     Asynchronous wrapper for a synchronous rule.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public class AsyncRuleWrapper<T> : IAsyncRule<T>
    {
        private readonly IRule<T> _syncRule;

        /// <summary>
        ///     Create a wrapper around the equivalent synchronous rule.
        /// </summary>
        /// <param name="syncRule">The synchronous rule.</param>
        public AsyncRuleWrapper(IRule<T> syncRule) => _syncRule = syncRule;

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