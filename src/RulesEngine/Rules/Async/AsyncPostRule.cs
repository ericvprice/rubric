namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     An asynchronous postprocessing rule.
    /// </summary>
    /// <typeparam name="TOut">The engine output type.</typeparam>
    public abstract class AsyncPostRule<TOut> : AsyncPrePostRule<TOut>, IAsyncPostRule<TOut>
    {
    }
}