using System.Threading;

namespace Rubric.Scripting;

public class ScriptedRuleContext<T>
{

  public ScriptedRuleContext(IEngineContext context, T input, CancellationToken t)
  {
    Input = input;
    Context = context;
    Token = t;
  }

  public T Input { get; }

  public IEngineContext Context { get; }

  public CancellationToken Token { get; }
}

public class ScriptedRuleContext<T, U> : ScriptedRuleContext<T>
{

  public ScriptedRuleContext(IEngineContext context, T input, U output, CancellationToken t)
  : base(context, input, t)
  {
    Output = output;
  }

  public U Output { get; }
}
