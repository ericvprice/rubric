using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine.Builder
{
    internal class AsyncEngineBuilder<TIn, TOut> : IAsyncEngineBuilder<TIn, TOut>
    {
        private readonly ILogger _logger;

        public AsyncEngineBuilder(ILogger logger = null) => _logger = logger ?? NullLogger.Instance;

        internal AsyncRuleset<TIn, TOut> AsyncRuleset { get; } = new AsyncRuleset<TIn, TOut>();

        public IAsyncRulesEngine<TIn, TOut> Build()
            => new AsyncRulesEngine<TIn, TOut>(AsyncRuleset, _logger);

        public IAsyncPostRuleBuilder<TIn, TOut> WithPostRule(string name)
            => new AsyncPostRuleBuilder<TIn, TOut>(this, name);

        public IAsyncEngineBuilder<TIn, TOut> WithPostRule(IAsyncPostRule<TOut> rule)
        {
            AsyncRuleset.AddAsyncPostRule(rule);
            return this;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> WithPreRule(string name)
            => new AsyncPreRuleBuilder<TIn, TOut>(this, name);

        public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IAsyncPreRule<TIn> rule)
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

        public IAsyncEngineBuilder<TIn, TOut> WithPostRule(IPostRule<TOut> rule)
        {
            AsyncRuleset.AddAsyncPostRule(rule.WrapAsync());
            return this;
        }

        public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IPreRule<TIn> rule)
        {
            AsyncRuleset.AddAsyncPreRule(rule.WrapAsync());
            return this;
        }

        public IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
        {
            AsyncRuleset.AddAsyncRule(rule.WrapAsync());
            return this;
        }
    }
}