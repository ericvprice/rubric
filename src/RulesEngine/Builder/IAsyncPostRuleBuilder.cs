namespace RulesEngine.Builder
{
    public interface IAsyncPostRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IAsyncPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, Task<bool>> predicate);

        IAsyncPostRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TOut, Task> action);

        IAsyncPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IAsyncPostRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

        IAsyncPostRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IAsyncEngineBuilder<TIn, TOut> EndRule();
    }
}