namespace Rubric.Rules;

/// <summary>
///   Utility rule that only runs if the DEBUG symbol is defined.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public abstract class DebugRule<T> : Rule<T>
{
  /// <inheritdoc />
  public override bool DoesApply(IEngineContext context, T input)
    #if DEBUG
      => true;
    #else
      => false;
    #endif
}