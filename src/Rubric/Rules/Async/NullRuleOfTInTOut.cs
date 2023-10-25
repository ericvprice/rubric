namespace Rubric.Rules.Async;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public class NullRule<TIn, TOut> : Rule<TIn, TOut>
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => Task.CompletedTask;

  /// <inheritdoc />
  public override Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => Task.FromResult(false);
}