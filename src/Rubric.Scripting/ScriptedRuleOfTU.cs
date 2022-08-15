using Microsoft.CodeAnalysis.Scripting;
using static Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;
using static Rubric.Scripting.ScriptingHelpers;
namespace Rubric.Scripting;

public class ScriptedRule<T, U> : IAsyncRule<T, U>
{

  private static readonly Type CONTEXT_TYPE = typeof(ScriptedRuleContext<T, U>);
  private const string DOES_APPLY_TRAILER = "return DoesApply(Context, Input, Output, Token);";
  private const string APPLY_TRAILER = "return Apply(Context, Input, Output, Token);";

  private readonly ScriptRunner<Task<bool>> _doesApply;
  private readonly ScriptRunner<Task> _apply;

  public ScriptedRule(
    string name,
    string script,
    string[] dependsOn = null,
    string[] provides = null,
    ScriptOptions options = default)
  {
    Dependencies = dependsOn ?? new string[] { };
    Provides = provides ?? new string[] { };
    Name = name;
    options ??= GetDefaultOptions<T, U>();
    var baseScript = Create<bool>(script.FilterScript(),
                                 options,
                                 globalsType: CONTEXT_TYPE);
    _doesApply = baseScript.ContinueWith<Task<bool>>(DOES_APPLY_TRAILER)
                           .CreateDelegate();
    _apply = baseScript.ContinueWith<Task>(APPLY_TRAILER)
                           .CreateDelegate();
  }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public string Name { get; }

  public async Task Apply(IEngineContext context, T input, U output, CancellationToken t)
      => await await _apply(new ScriptedRuleContext<T, U>(context, input, output, t));

  public async Task<bool> DoesApply(IEngineContext context, T input, U output, CancellationToken t)
      => await await _doesApply(new ScriptedRuleContext<T, U>(context, input, output, t));
}
