namespace RulesEngine.Rules.Async 
{

    /// <summary>
    ///     An asynchronous postprocessing rule for engine outputs.
    /// </summary>
    /// <typeparam name="TOut">The engine output type.</typeparam>
    public interface IAsyncPostRule<in TOut> : IAsyncPrePostRule<TOut> {}

}