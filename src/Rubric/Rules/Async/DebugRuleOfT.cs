namespace Rubric.Rules.Async;

/// <summary>
///   Utility rule that only runs if the DEBUG symbol is defined.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public abstract class DebugRule<T> : Rule<T> where T : class
{
  /// <inheritdoc />
  public override Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
#if DEBUG
      => Task.FromResult(true);
#else
      => Task.FromResult(false);
#endif
}