using Rubric.Dependency;

namespace Rubric.Rules.Probabilistic;

/// <summary>
///     Abstract rule suitable for extension using attributes
///     for declarative dependencies.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public abstract class Rule<TIn, TOut> : BaseDependency, IRule<TIn, TOut>
{
  /// <inheritdoc />
  public override string Name => GetType().FullName;

  /// <inheritdoc />
  public abstract void Apply(IEngineContext context, TIn input, TOut output);

  /// <inheritdoc />
  public abstract double DoesApply(IEngineContext context, TIn input, TOut output);
}