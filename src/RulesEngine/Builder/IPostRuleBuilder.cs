using System;

namespace RulesEngine.Builder
{
    public interface IPostRuleBuilder<TIn, TOut>
    {
        IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, bool> predicate);

        IPostRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TOut> action);

        IPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IPostRuleBuilder<TIn, TOut> ThatDependsOn(Type type);

        IPostRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IEngineBuilder<TIn, TOut> EndRule();
    }
}