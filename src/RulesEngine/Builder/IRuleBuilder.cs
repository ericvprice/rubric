namespace RulesEngine.Builder
{
    public interface IRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, bool> predicate);

        IRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn, TOut> action);

        IRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

        IRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IEngineBuilder<TIn, TOut> EndRule();
    }

    public interface IRuleBuilder<T>
        where T : class
    {
        IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, bool> predicate);

        IRuleBuilder<T> WithAction(Action<IEngineContext, T> action);

        IRuleBuilder<T> ThatDependsOn(string dep);

        IRuleBuilder<T> ThatDependsOn(Type dep);

        IRuleBuilder<T> ThatProvides(string provides);

        IEngineBuilder<T> EndRule();
    }
}