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
  public ScriptedRuleContext(IEngineContext context, TIn input, TOut output, CancellationToken t)
    : base(context, input, t)
    => Output = output;

  /// <summary>
  ///   The output object.
  /// </summary>
  /// <value>The output object.</value>
  public TOut Output { get; }
}