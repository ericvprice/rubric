namespace Rubric.Rules.Probabilistic.Async;

/// <summary>
///   Asynchronous wrapper for a synchronous rule.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public class AsyncRuleWrapper<T> : IRule<T>
{
  private readonly Probabilistic.IRule<T> _syncRule;

  /// <summary>
  ///   Create a wrapper around the equivalent synchronous rule.
  /// </summary>
  /// <param name="syncRule">The synchronous rule.</param>
  public AsyncRuleWrapper(Probabilistic.IRule<T> syncRule) => _syncRule = syncRule;

  /// <inheritdoc />
  public Task Apply(IEngineContext context, T input, CancellationToken token)
  {
    token.ThrowIfCancellationRequested();
    _syncRule.Apply(context, input);
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public Task<double> DoesApply(IEngineContext context, T input, CancellationToken token)
  {
    token.ThrowIfCancellationRequested();
    return Task.FromResult(_syncRule.DoesApply(context, input));
  }


  /// <inheritdoc />
  public string Name => _syncRule.Name + " (wrapped async)";

  /// <inheritdoc />
  public IEnumerable<string> Dependencies => _syncRule.Dependencies;

  /// <inheritdoc />
  public IEnumerable<string> Provides => _syncRule.Provides;
}