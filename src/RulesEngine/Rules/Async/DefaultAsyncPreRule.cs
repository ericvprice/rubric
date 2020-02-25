using System.Threading.Tasks;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     An asynchronous preprocessing rule that is always executed.
    /// </summary>
    /// <typeparam name="TIn">The engine input.</typeparam>
    public abstract class DefaultAsyncPreRule<TIn> : AsyncPreRule<TIn>
    {
        public override Task<bool> DoesApply(IEngineContext context, TIn obj) => Task.FromResult(true);
    }
}