using System;

namespace RulesEngine.Builder
{
    public interface IRuleBuilder<TIn, TOut>
    {

        IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, bool> predicate);

        IRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn, TOut> action);

        IRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

        IRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IEngineBuilder<TIn, TOut> EndRule();
    }
}