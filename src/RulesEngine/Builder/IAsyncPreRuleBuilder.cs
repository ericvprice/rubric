using System;
using System.Threading;
using System.Threading.Tasks;

namespace RulesEngine.Builder
{
    public interface IAsyncPreRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IAsyncPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, Task<bool>> predicate);

        IAsyncPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, CancellationToken, Task<bool>> predicate);

        IAsyncPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, Task> action);

        IAsyncPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, CancellationToken, Task> action);

        IAsyncPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IAsyncPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

        IAsyncPreRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IAsyncEngineBuilder<TIn, TOut> EndRule();
    }
}