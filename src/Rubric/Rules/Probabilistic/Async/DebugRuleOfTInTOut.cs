namespace Rubric.Rules.Probabilistic.Async;

/// <summary>
///   Utility rule that only runs if the DEBUG symbol is defined.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type</typeparam>
public abstract class DebugRule<TIn, TOut> : Rule<TIn, TOut>
{
  /// <inheritdoc />
  public override Task<double> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    #if DEBUG
        => Task.FromResult(1D);
    #else
        => Task.FromResult(0D);
    #endif
}