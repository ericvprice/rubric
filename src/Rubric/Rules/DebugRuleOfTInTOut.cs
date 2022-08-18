namespace Rubric.Rules;

/// <summary>
///   Utility rule that only runs if the DEBUG symbol is defined.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type</typeparam>
public abstract class DebugRule<TIn, TOut> : Rule<TIn, TOut>
{
  /// <inheritdoc />
  public override bool DoesApply(IEngineContext context, TIn input, TOut output)
    #if DEBUG
    => true;
  #else
      => false;
  #endif
}