namespace RulesEngine.Rules
{
    /// <summary>
    ///     Convenience naming for clarity.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    public abstract class PreRule<TIn> : PrePostRule<TIn>, IPreRule<TIn>
    { 
    }
}