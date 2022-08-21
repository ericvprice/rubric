namespace Rubric.Rules;

/// <summary>
///     A rule that is always executed.
/// </summary>
/// <typeparam name="T">The engine input.</typeparam>
public abstract class DefaultRule<T> : Rule<T>
{
  public override bool DoesApply(IEngineContext context, T input) => true;
}
