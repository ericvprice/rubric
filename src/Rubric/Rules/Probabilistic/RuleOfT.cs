using Rubric.Dependency;

namespace Rubric.Rules.Probabilistic;

/// <summary>
///     Abstract rule suitable for extension using attributes
///     for declarative dependencies.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public abstract class Rule<T> : BaseDependency, IRule<T>
{
  /// <inheritdoc />
  public override string Name => GetType().FullName;

  /// <inheritdoc />
  public abstract void Apply(IEngineContext context, T input);

  /// <inheritdoc />
  public abstract double DoesApply(IEngineContext context, T input);
}