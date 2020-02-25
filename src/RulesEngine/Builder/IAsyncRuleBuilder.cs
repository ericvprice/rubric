using System;
using System.Threading.Tasks;

namespace RulesEngine.Builder
{
    public interface IAsyncRuleBuilder<TIn, TOut>
    {
        IAsyncRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, Task<bool>> predicate);

        IAsyncRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, Task> action);

        IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

        IAsyncRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IAsyncEngineBuilder<TIn, TOut> EndRule();
    }
}