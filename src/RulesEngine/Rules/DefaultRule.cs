namespace RulesEngine.Rules;

/// <summary>
///     A rule that is always executed.
/// </summary>
/// <typeparam name="TIn">The engine input.</typeparam>
/// <typeparam name="TOut">The engine output.</typeparam>
public abstract class DefaultRule<TIn, TOut> : Rule<TIn, TOut>
{
  public override bool DoesApply(IEngineContext context, TIn input, TOut output) => true;
}

/// <summary>
///     A rule that is always executed.
/// </summary>
/// <typeparam name="T">The engine input.</typeparam>
public abstract class DefaultRule<T> : Rule<T>
{
  public override bool DoesApply(IEngineContext context, T input) => true;
}
