namespace RulesEngine.Builder
{
    public interface IPreRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, bool> predicate);

        IPreRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn> action);

        IPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

        IPreRuleBuilder<TIn, TOut> ThatProvides(string provides);

        IEngineBuilder<TIn, TOut> EndRule();

        IPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);
    }
}