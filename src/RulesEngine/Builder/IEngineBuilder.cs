using RulesEngine.Rules;

namespace RulesEngine.Builder
{
    public interface IEngineBuilder<TIn, TOut>
    {
        IPreRuleBuilder<TIn, TOut> WithPreRule(string name);

        IRuleBuilder<TIn, TOut> WithRule(string name);

        IPostRuleBuilder<TIn, TOut> WithPostRule(string name);

        IEngineBuilder<TIn, TOut> WithPreRule(IPreRule<TIn> rule);

        IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

        IEngineBuilder<TIn, TOut> WithPostRule(IPostRule<TOut> rule);

        IRulesEngine<TIn, TOut> Build();
    }
}