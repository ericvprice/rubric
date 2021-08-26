namespace RulesEngine.Builder
{
    public interface IAsyncRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IAsyncRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, Task<bool>> predicate);

        IAsyncRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, Task> action);

        IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

        IAsyncRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IAsyncEngineBuilder<TIn, TOut> EndRule();
    }

    public interface IAsyncRuleBuilder<T>
        where T : class
    {
        IAsyncRuleBuilder<T> WithPredicate(Func<IEngineContext, T, Task<bool>> predicate);

        IAsyncRuleBuilder<T> WithAction(Func<IEngineContext, T, Task> action);

        IAsyncRuleBuilder<T> ThatDependsOn(string dep);

        IAsyncRuleBuilder<T> ThatDependsOn(Type dep);

        IAsyncRuleBuilder<T> ThatProvides(string provides);

        IAsyncEngineBuilder<T> EndRule();
    }
}