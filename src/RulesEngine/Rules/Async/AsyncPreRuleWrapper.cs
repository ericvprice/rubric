namespace RulesEngine.Rules.Async
{
    public class AsyncPreRuleWrapper<TIn> : AsyncPrePostRuleWrapper<TIn>, IAsyncPreRule<TIn>
    {
        public AsyncPreRuleWrapper(IPrePostRule<TIn> syncRule) : base(syncRule) { }
    }
}