namespace RulesEngine.Rules
{
    /// <summary>
    ///     A postprocessing rule for engine outputs.
    /// </summary>
    /// <typeparam name="TOut">The output type for the engine.</typeparam>
    public interface IPostRule<in TOut> : IPrePostRule<TOut> {}
}
