using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace Rubric.Scripting;

public class ScriptedRule<T, U> : IAsyncRule<T, U>
{
  private readonly ScriptRunner<bool> _doesApply;
  private readonly ScriptRunner<object> _apply;

  public ScriptedRule(
    string name,
    string doesApplyScript,
    string applyScript,
    string[] dependsOn = null,
    string[] provides = null,
    ScriptOptions options = default)
  {
    Dependencies = dependsOn ?? new string[] { };
    Provides = provides ?? new string[] { };
    Name = name;
    options ??= GetDefaultOptions();
    _doesApply = CSharpScript.Create<bool>(doesApplyScript,
                                           options,
                                           globalsType: typeof(ScriptedRuleContext<T, U>))
                             .CreateDelegate();
    _apply = CSharpScript.Create(applyScript,
                                 options,
                                 globalsType: typeof(ScriptedRuleContext<T, U>))
                         .CreateDelegate();
  }

  private ScriptOptions GetDefaultOptions()
   => ScriptOptions.Default
                   .WithReferences(typeof(ScriptedRuleContext<T, U>).Assembly)
                   .WithReferences(typeof(EngineContext).Assembly)
                   .WithReferences(typeof(ILogger).Assembly)
                   .WithReferences(typeof(T).Assembly)
                   .WithReferences(typeof(U).Assembly);

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public string Name { get; }

  public async Task Apply(IEngineContext context, T input, U output, CancellationToken t)
      => await _apply(new ScriptedRuleContext<T, U>(context, input, output, t));

  public async Task<bool> DoesApply(IEngineContext context, T input, U output, CancellationToken t)
      => await _doesApply(new ScriptedRuleContext<T, U>(context, input, output, t));
}
