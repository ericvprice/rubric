using System.Threading.Tasks;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     A postprocessing rule that is always executed.
    /// </summary>
    /// <typeparam name="TOut">The engine output.</typeparam>
    public abstract class DefaultAsyncPostRule<TOut> : AsyncPostRule<TOut>
    {
        public override Task<bool> DoesApply(IEngineContext context, TOut obj) => Task.FromResult(true);
    }
}