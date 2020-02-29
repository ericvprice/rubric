using RulesEngine.Rules;

namespace RulesEngine.Builder
{
    public interface IEngineBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IPreRuleBuilder<TIn, TOut> WithPreRule(string name);

        IRuleBuilder<TIn, TOut> WithRule(string name);

        IPostRuleBuilder<TIn, TOut> WithPostRule(string name);

        IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

        IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

        IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

        IRulesEngine<TIn, TOut> Build();
    }

    public interface IEngineBuilder<T>
        where T : class
    {
        IRuleBuilder<T> WithRule(string name);

        IEngineBuilder<T> WithRule(IRule<T> rule);

        IRulesEngine<T> Build();
    }
}