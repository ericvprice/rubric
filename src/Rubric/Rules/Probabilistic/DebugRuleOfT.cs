namespace Rubric.Rules.Probabilistic;

/// <summary>
///   Utility rule that only runs if the DEBUG symbol is defined.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public abstract class DebugRule<T> : Rule<T>
{
  /// <inheritdoc />
  public override double DoesApply(IEngineContext context, T input)
    #if DEBUG
      => 1;
    #else
      => 0;
    #endif
}