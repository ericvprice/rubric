using System.Threading;
using Rubric.Dependency;

namespace Rubric.Rules.Async;

/// <summary>
///     An asynchronous processing rule.
/// </summary>
/// <typeparam name="T">The object type.</typeparam>
public abstract class AsyncRule<T> : BaseDependency, IAsyncRule<T>
      where T : class
{
  /// <inheritdoc />
  public override string Name => GetType().FullName;

  /// <inheritdoc />
  public abstract Task Apply(IEngineContext context, T input, CancellationToken token);
  /// <inheritdoc />
  public abstract Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token);

}
