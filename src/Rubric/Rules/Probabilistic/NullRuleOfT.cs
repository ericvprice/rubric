using System.Diagnostics.CodeAnalysis;

namespace Rubric.Rules.Probabilistic;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public class NullRule<T> : Rule<T>
{
  /// <inheritdoc />
  [ExcludeFromCodeCoverage]
  public override void Apply(IEngineContext context, T input) { }

  /// <inheritdoc />
  public override double DoesApply(IEngineContext context, T input) => 0;
}