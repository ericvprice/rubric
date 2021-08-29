namespace RulesEngine.Rules.Async;

/// <summary>
///     Asynchronous wrapper for a synchronous rule.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public class AsyncRuleWrapper<TIn, TOut> : IAsyncRule<TIn, TOut>
{
  private readonly IRule<TIn, TOut> _syncRule;

  /// <summary>
  ///     Create a wrapper around the equivalent synchronous rule.
  /// </summary>
  /// <param name="syncRule">The synchronous rule.</param>
  public AsyncRuleWrapper(IRule<TIn, TOut> syncRule) => _syncRule = syncRule;

  /// <inheritdoc />
  public Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token)
  {
    token.ThrowIfCancellationRequested();
    _syncRule.Apply(context, input, output);
    return Task.CompletedTask;
  }

  /// <inheritdoc />
  public Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
  {
    token.ThrowIfCancellationRequested();
    return Task.FromResult(_syncRule.DoesApply(context, input, output));
  }


  /// <inheritdoc />
  public string Name => _syncRule.Name + " (wrapped async)";

  /// <inheritdoc />
  public IEnumerable<string> Dependencies => _syncRule.Dependencies;

  /// <inheritdoc />
  public IEnumerable<string> Provides => _syncRule.Provides;
}
