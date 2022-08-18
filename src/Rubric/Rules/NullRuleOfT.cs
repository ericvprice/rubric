namespace Rubric.Rules;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public class NullRule<T> : Rule<T>
{
  /// <inheritdoc />
  public override void Apply(IEngineContext context, T input) { }

  /// <inheritdoc />
  public override bool DoesApply(IEngineContext context, T input)
    => false;
}