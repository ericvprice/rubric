namespace Rubric.Scripting;

public class ScriptedRuleContext<T, U> : ScriptedRuleContext<T>
{

  public ScriptedRuleContext(IEngineContext context, T input, U output, CancellationToken t)
  : base(context, input, t)
  {
    Output = output;
  }

  public U Output { get; }
}
