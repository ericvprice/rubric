namespace Rubric.Rules.Probabilistic.Async;

/// <summary>
///   An asynchronous rule that is always executed.
/// </summary>
/// <typeparam name="T">The engine input.</typeparam>
public abstract class DefaultRule<T> : Rule<T>
  where T : class
{
  /// <inheritdoc />
  public override Task<double> DoesApply(IEngineContext context, T input, CancellationToken token)
    => Task.FromResult(1D);
}