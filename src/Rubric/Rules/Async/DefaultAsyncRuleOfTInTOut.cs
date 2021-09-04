namespace Rubric.Rules.Async;

/// <summary>
///     An asynchronous rule that is always executed.
/// </summary>
/// <typeparam name="TIn">The engine input.</typeparam>
/// <typeparam name="TOut">The engine output.</typeparam>
public abstract class DefaultAsyncRule<TIn, TOut> : AsyncRule<TIn, TOut>
    where TIn : class
    where TOut : class
{

  /// <inheritdoc />
  public override Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token) => Task.FromResult(true);


}
