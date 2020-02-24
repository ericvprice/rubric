namespace RulesEngine.Rules.Async
{
    public class AsyncPostRuleWrapper<TOut> : AsyncPrePostRuleWrapper<TOut>, IAsyncPostRule<TOut>
    {
        public AsyncPostRuleWrapper(IPrePostRule<TOut> syncRule) : base(syncRule)
        {
        }
    }
}