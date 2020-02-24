
namespace RulesEngine.Rules
{
    /// <summary>
    ///     A preprocessing rule for engine inputs.
    /// </summary>
    /// <typeparam name="TIn">The input type for the engine.</typeparam>
    public interface IPreRule<in TIn> : IPrePostRule<TIn>
    {
    }
}
