using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rules.Async;

namespace Rubric.Scripting
{

  public class ScriptedRule<T> : IAsyncRule<T>
  {
    private readonly ScriptRunner<bool> _doesApply;
    private readonly ScriptRunner<object> _apply;

    public ScriptedRule(
      string name,
      string doesApplyScript,
      string applyScript,
      string[] dependsOn = null,
      string[] provides = null
    )
    {
      Name = name;
      Provides = provides ?? new string[] { };
      Dependencies = dependsOn ?? new string[] { };
      _doesApply = CSharpScript.Create<bool>(doesApplyScript, globalsType: typeof(ScriptedRuleContext<T>))
                               .CreateDelegate();
      _apply = CSharpScript.Create(applyScript, globalsType: typeof(ScriptedRuleContext<T>))
                           .CreateDelegate();
    }

    public IEnumerable<string> Dependencies { get; }

    public IEnumerable<string> Provides { get; }

    public string Name { get; }

    public async Task Apply(IEngineContext context, T input, CancellationToken t)
        => await _apply(new ScriptedRuleContext<T>(context, input, t));

    public async Task<bool> DoesApply(IEngineContext context, T input, CancellationToken t)
        => await _doesApply(new ScriptedRuleContext<T>(context, input, t));
  }
}
