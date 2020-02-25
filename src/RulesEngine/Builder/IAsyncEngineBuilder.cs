using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine.Builder
{
    public interface IAsyncEngineBuilder<TIn, TOut>
    {
        IAsyncEngineBuilder<TIn, TOut> WithPreRule(IPreRule<TIn> rule);

        IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

        IAsyncEngineBuilder<TIn, TOut> WithPostRule(IPostRule<TOut> rule);

        IAsyncPreRuleBuilder<TIn, TOut> WithPreRule(string name);

        IAsyncRuleBuilder<TIn, TOut> WithRule(string name);

        IAsyncPostRuleBuilder<TIn, TOut> WithPostRule(string name);

        IAsyncEngineBuilder<TIn, TOut> WithPreRule(IAsyncPreRule<TIn> rule);

        IAsyncEngineBuilder<TIn, TOut> WithRule(IAsyncRule<TIn, TOut> rule);

        IAsyncEngineBuilder<TIn, TOut> WithPostRule(IAsyncPostRule<TOut> rule);

        IAsyncEngineBuilder<TIn, TOut> AsParallel();

        IAsyncRulesEngine<TIn, TOut> Build();
    }
}