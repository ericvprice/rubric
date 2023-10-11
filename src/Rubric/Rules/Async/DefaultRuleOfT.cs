namespace Rubric.Rules.Async;

/// <summary>
///   An asynchronous rule that is always executed.
/// </summary>
/// <typeparam name="T">The engine input.</typeparam>
public abstract class DefaultAsyncRule<T> : Rule<T>
  where T : class
{
  /// <inheritdoc />
  public override Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
    => Task.FromResult(true);
}