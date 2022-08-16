namespace Rubric.Rules.Scripted;

public class ScriptedRuleContext<TIn, TOut> : ScriptedRuleContext<TIn>
{
  public ScriptedRuleContext(IEngineContext context, TIn input, TOut output, CancellationToken t)
    : base(context, input, t)
    => Output = output;

  public TOut Output { get; }
}
