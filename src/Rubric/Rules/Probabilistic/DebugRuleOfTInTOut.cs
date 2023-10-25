namespace Rubric.Rules.Probabilistic;

/// <summary>
///   Utility rule that only runs if the DEBUG symbol is defined.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type</typeparam>
public abstract class DebugRule<TIn, TOut> : Rule<TIn, TOut>
{
  /// <inheritdoc />
  public override double DoesApply(IEngineContext context, TIn input, TOut output)
    #if DEBUG
        => 1;
    #else
    => 0;
  #endif
}