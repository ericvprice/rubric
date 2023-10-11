namespace Rubric.Rules.Probabilistic;

/// <summary>
///     A rule that is always executed.
/// </summary>
/// <typeparam name="TIn">The engine input.</typeparam>
/// <typeparam name="TOut">The engine output.</typeparam>
public abstract class DefaultRule<TIn, TOut> : Rule<TIn, TOut>
{
  /// <inheritdoc/>
  public override double DoesApply(IEngineContext context, TIn input, TOut output) => 1;
}