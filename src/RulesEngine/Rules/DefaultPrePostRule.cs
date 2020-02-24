namespace RulesEngine.Rules
{
    /// <summary>
    ///     Convenience superclass for default pre/post rules.
    /// </summary>
    /// <typeparam name="T">The input/output type.</typeparam>
    public abstract class DefaultPrePostRule<T> : PrePostRule<T> {
        
        /// <summary>
        ///     Always return true.
        /// </summary>
        /// <param name="context">The engine context.</param>
        /// <param name="obj">The object.</param>
        /// <returns>True.</returns>
        public override bool DoesApply(IEngineContext context, T obj) => true;
    
    }

}