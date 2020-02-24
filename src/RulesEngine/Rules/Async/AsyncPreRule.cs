namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     An asynchronous preprocessing rule.
    /// </summary>
    /// <typeparam name="TIn">The engine input type.</typeparam>
    public abstract class AsyncPreRule<TIn> : AsyncPrePostRule<TIn>, IAsyncPreRule<TIn>
    {
    }
}