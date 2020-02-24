namespace RulesEngine.Rules
{
    /// <summary>
    ///     Convenience naming for clarity.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    public abstract class PostRule<TOut> : PrePostRule<TOut>, IPostRule<TOut>
    {
    }
}