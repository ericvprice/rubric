using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine.Builder
{
    public interface IAsyncEngineBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IAsyncEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

        IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

        IAsyncEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

        IAsyncPreRuleBuilder<TIn, TOut> WithPreRule(string name);

        IAsyncRuleBuilder<TIn, TOut> WithRule(string name);

        IAsyncPostRuleBuilder<TIn, TOut> WithPostRule(string name);

        IAsyncEngineBuilder<TIn, TOut> WithPreRule(IAsyncRule<TIn> rule);

        IAsyncEngineBuilder<TIn, TOut> WithRule(IAsyncRule<TIn, TOut> rule);

        IAsyncEngineBuilder<TIn, TOut> WithPostRule(IAsyncRule<TOut> rule);

        IAsyncEngineBuilder<TIn, TOut> AsParallel();

        IAsyncRulesEngine<TIn, TOut> Build();
    }

    public interface IAsyncEngineBuilder<T>
        where T : class
    {
        IAsyncEngineBuilder<T> WithRule(IRule<T> rule);

        IAsyncRuleBuilder<T> WithRule(string name);

        IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule);

        IAsyncEngineBuilder<T> AsParallel();

        IAsyncRulesEngine<T> Build();
    }
}