namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     An asynchronous preprocessing rule for engine inputs.
    /// </summary>
    /// <typeparam name="TIn">The engine output type.</typeparam>
    public interface IAsyncPreRule<in TIn> : IAsyncPrePostRule<TIn> { }
}