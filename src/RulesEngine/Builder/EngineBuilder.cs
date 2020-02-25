using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Rules;

namespace RulesEngine.Builder
{
    internal class EngineBuilder<TIn, TOut> : IEngineBuilder<TIn, TOut>
    {
        internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

        internal Ruleset<TIn, TOut> Ruleset { get; } = new Ruleset<TIn, TOut>();

        internal ILogger Logger { get; }

        public IPostRuleBuilder<TIn, TOut> WithPostRule(string name)
            => new PostRuleBuilder<TIn, TOut>(this, name);

        public IPreRuleBuilder<TIn, TOut> WithPreRule(string name)
            => new PreRuleBuilder<TIn, TOut>(this, name);

        public IRuleBuilder<TIn, TOut> WithRule(string name)
            => new RuleBuilder<TIn, TOut>(this, name);

        public IEngineBuilder<TIn, TOut> WithPostRule(IPostRule<TOut> rule)
        {
            Ruleset.AddPostRule(rule);
            return this;
        }

        public IEngineBuilder<TIn, TOut> WithPreRule(IPreRule<TIn> rule)
        {
            Ruleset.AddPreRule(rule);
            return this;
        }

        public IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
        {
            Ruleset.AddRule(rule);
            return this;
        }

        public IRulesEngine<TIn, TOut> Build() => new RulesEngine<TIn, TOut>(Ruleset, Logger);
    }

    public static class EngineBuilder
    {
        public static IEngineBuilder<TIn, TOut> ForInputAndOutput<TIn, TOut>(ILogger logger = null)
            => new EngineBuilder<TIn, TOut>(logger);

        public static IAsyncEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(ILogger logger = null)
            => new AsyncEngineBuilder<TIn, TOut>(logger);
    }
}