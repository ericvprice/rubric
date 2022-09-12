namespace Rubric.Rules.Scripted;

/// <summary>
///   A context object passed to the dynamically executed rule execution.
/// </summary>
/// <typeparam name="T">The rule type.</typeparam>
public class ScriptedRuleContext<T>
{

  public ScriptedRuleContext(IEngineContext context, T input, CancellationToken t)
  {
    Input = input;
    Context = context;
    Token = t;
  }

  /// <summary>
  ///   The input object
  /// </summary>
  /// <value>The input object.</value>
  public T Input { get; }

  /// <summary>
  ///   The engine context.
  /// </summary>
  /// <value>The engine context.</value>
  public IEngineContext Context { get; }

  /// <summary>
  ///   The execution token.
  /// </summary>
  /// <value>The execution token.</value>
  public CancellationToken Token { get; }

}