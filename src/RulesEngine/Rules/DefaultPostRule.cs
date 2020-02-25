namespace RulesEngine.Rules
{
    /// <summary>
    ///     A postprocessing rule that is always executed.
    /// </summary>
    /// <typeparam name="TOut">The engine output.</typeparam>
    public abstract class DefaultPostRule<TOut> : DefaultPrePostRule<TOut>, IPostRule<TOut> { }
}