using Rubric.Dependency;

namespace Rubric.Rules.Async;

/// <summary>
///   A runtime-constructed asynchronous processing rule.
/// </summary>
/// <typeparam name="TIn">The engine input type.</typeparam>
/// <typeparam name="TOut">The engine output type.</typeparam>
public abstract class AsyncRule<TIn, TOut> : BaseDependency, IAsyncRule<TIn, TOut>
{
  /// <inheritdoc />
  public override string Name => GetType().FullName;

  /// <inheritdoc />
  public abstract Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token);

  /// <inheritdoc />
  public abstract Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token);
}