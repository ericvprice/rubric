using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace Rubric.Scripting;

public class ScriptedRule<T> : IAsyncRule<T>
{
  private readonly ScriptRunner<bool> _doesApply;
  private readonly ScriptRunner<object> _apply;

  public ScriptedRule(
    string name,
    string doesApplyScript,
    string applyScript,
    string[] dependsOn = null,
    string[] provides = null,
    ScriptOptions options = default
  )
  {
    Name = name;
    Provides = provides?.Append(Name).ToArray() ?? new string[] { Name };
    Dependencies = dependsOn ?? new string[] { };
    options ??= GetDefaultOptions();
    _doesApply = CSharpScript.Create<bool>(doesApplyScript,
                                           options,
                                           globalsType: typeof(ScriptedRuleContext<T>))
                             .CreateDelegate();
    _apply = CSharpScript.Create(applyScript,
                                 options,
                                 globalsType: typeof(ScriptedRuleContext<T>))
                         .CreateDelegate();
  }

  private ScriptOptions GetDefaultOptions()
   => ScriptOptions.Default
                   .WithReferences(typeof(ScriptedRuleContext<T>).Assembly)
                   .WithReferences(typeof(EngineContext).Assembly)
                   .WithReferences(typeof(ILogger).Assembly)
                   .WithReferences(typeof(T).Assembly);

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public string Name { get; }

  public async Task Apply(IEngineContext context, T input, CancellationToken t)
      => await _apply(new ScriptedRuleContext<T>(context, input, t));

  public async Task<bool> DoesApply(IEngineContext context, T input, CancellationToken t)
      => await _doesApply(new ScriptedRuleContext<T>(context, input, t));
}
