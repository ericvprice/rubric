namespace RulesEngine.Rules
{
    /// <summary>
    ///     A preprocessing rule that is always executed.
    /// </summary>
    /// <typeparam name="TIn">The engine input.</typeparam>
    public abstract class DefaultPreRule<TIn> : DefaultPrePostRule<TIn>, IPreRule<TIn> { }
}