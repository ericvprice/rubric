using System.Diagnostics.CodeAnalysis;

namespace Rubric.Rules.Scripted;

/// <summary>
///   A context object passed to the dynamically executed rule execution.
/// </summary>
/// <typeparam name="T">The rule type.</typeparam>
#pragma warning disable IDE0079 // Remove unnecessary suppression
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
public class ScriptedRuleContext<T>
{

  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="context">The current engine execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="token">The cancellation token to use.</param>
  public ScriptedRuleContext(IEngineContext context, T input, CancellationToken token)
  {
    Input = input;
    Context = context;
    Token = token;
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