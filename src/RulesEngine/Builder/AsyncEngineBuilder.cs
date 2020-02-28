using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine.Builder
{
    internal class AsyncEngineBuilder<TIn, TOut> : IAsyncEngineBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        private readonly ILogger _logger;

        private bool _isParallel;

        public AsyncEngineBuilder(ILogger logger = null) => _logger = logger ?? NullLogger.Instance;

        internal AsyncRuleset<TIn, TOut> AsyncRuleset { get; } = new AsyncRuleset<TIn, TOut>();

        public IAsyncRulesEngine<TIn, TOut> Build()
            => new AsyncRulesEngine<TIn, TOut>(AsyncRuleset, _isParallel, _logger);

        public IAsyncPostRuleBuilder<TIn, TOut> WithPostRule(string name)
            => new AsyncPostRuleBuilder<TIn, TOut>(this, name);

        public IAsyncEngineBuilder<TIn, TOut> WithPostRule(IAsyncRule<TOut> rule)
        {
            AsyncRuleset.AddAsyncPostRule(rule);
            return this;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> WithPreRule(string name)
            => new AsyncPreRuleBuilder<TIn, TOut>(this, name);

        public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IAsyncRule<TIn> rule)
        {
            AsyncRuleset.AddAsyncPreRule(rule);
            return this;
        }

        public IAsyncRuleBuilder<TIn, TOut> WithRule(string name)
            => new AsyncRuleBuilder<TIn, TOut>(this, name);

        public IAsyncEngineBuilder<TIn, TOut> WithRule(IAsyncRule<TIn, TOut> rule)
        {
            AsyncRuleset.AddAsyncRule(rule);
            return this;
        }

        public IAsyncEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule)
        {
            AsyncRuleset.AddAsyncPostRule(rule.WrapAsync());
            return this;
        }

        public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule)
        {
            AsyncRuleset.AddAsyncPreRule(rule.WrapAsync());
            return this;
        }

        public IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
        {
            AsyncRuleset.AddAsyncRule(rule.WrapAsync());
            return this;
        }

        public IAsyncEngineBuilder<TIn, TOut> AsParallel()
        {
            _isParallel = true;
            return this;
        }
    }

    internal class AsyncEngineBuilder<T> : IAsyncEngineBuilder<T>
        where T : class
    {
        private readonly ILogger _logger;

        private bool _isParallel;

        public AsyncEngineBuilder(ILogger logger = null) => _logger = logger ?? NullLogger.Instance;

        internal AsyncRuleset<T> AsyncRuleset { get; } = new AsyncRuleset<T>();

        public IAsyncRulesEngine<T> Build()
            => new AsyncRulesEngine<T>(AsyncRuleset, _isParallel, _logger);

        public IAsyncRuleBuilder<T> WithRule(string name)
            => new AsyncRuleBuilder<T>(this, name);

        public IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule)
        {
            AsyncRuleset.AddAsyncRule(rule);
            return this;
        }

        public IAsyncEngineBuilder<T> WithRule(IRule<T> rule)
        {
            AsyncRuleset.AddAsyncRule(rule.WrapAsync());
            return this;
        }

        public IAsyncEngineBuilder<T> AsParallel()
        {
            _isParallel = true;
            return this;
        }
    }
}