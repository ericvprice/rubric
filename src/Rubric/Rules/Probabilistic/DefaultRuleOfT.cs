namespace Rubric.Rules.Probabilistic;

/// <summary>
///     A rule that is always executed.
/// </summary>
/// <typeparam name="T">The engine input.</typeparam>
public abstract class DefaultRule<T> : Rule<T>
{
  /// <inheritdoc/>
  public override double DoesApply(IEngineContext context, T input) => 1;
}
