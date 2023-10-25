using Rubric.Dependency;

namespace Rubric.Rules.Probabilistic.Async;

/// <summary>
///   Abstract rule suitable for extension using attributes
///   for declarative dependencies.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public abstract class Rule<T> : BaseDependency, IRule<T>
{
  /// <inheritdoc />
  public virtual PredicateCaching CacheBehavior => GetType().GetPredicateCaching();

  /// <inheritdoc />
  public override string Name => GetType().FullName;

  /// <inheritdoc />
  public abstract Task Apply(IEngineContext context, T input, CancellationToken token);

  /// <inheritdoc />
  public abstract Task<double> DoesApply(IEngineContext context, T input, CancellationToken token);
}