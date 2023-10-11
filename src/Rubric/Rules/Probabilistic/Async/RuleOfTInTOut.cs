using Rubric.Dependency;

namespace Rubric.Rules.Probabilistic.Async;

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
  public abstract Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token);

  /// <inheritdoc />
  public abstract Task<double> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token);
}