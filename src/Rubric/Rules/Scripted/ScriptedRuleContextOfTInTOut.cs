using System.Diagnostics.CodeAnalysis;

namespace Rubric.Rules.Scripted;

/// <summary>
///   A context object passed to the dynamically executed rule execution.
/// </summary>
/// <typeparam name="TIn">The rule input type.</typeparam>
/// <typeparam name="TOut">The rule output type.</typeparam>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ScriptedRuleContext<TIn, TOut> : ScriptedRuleContext<TIn>
{
  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="context">The current engine execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  /// <param name="token">The cancellation token to use.</param>
  public ScriptedRuleContext(IEngineContext context, TIn input, TOut output, CancellationToken token)
    : base(context, input, token)
    => Output = output;

  /// <summary>
  ///   The output object.
  /// </summary>
  /// <value>The output object.</value>
  public TOut Output { get; }
}