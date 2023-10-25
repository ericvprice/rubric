namespace Rubric.Rules.Probabilistic.Async;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public class NullRule<T> : Rule<T> where T : class
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, T input, CancellationToken token)
    => Task.CompletedTask;

  /// <inheritdoc />
  public override Task<double> DoesApply(IEngineContext context, T input, CancellationToken token)
    => Task.FromResult(0D);

}