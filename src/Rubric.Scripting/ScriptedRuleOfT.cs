using Microsoft.CodeAnalysis.Scripting;
using static Rubric.Scripting.ScriptingHelpers;
using static Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;
namespace Rubric.Scripting;

public class ScriptedRule<T> : IAsyncRule<T>
{
  private static readonly Type CONTEXT_TYPE = typeof(ScriptedRuleContext<T>);
  private const string DOES_APPLY_TRAILER = "return DoesApply(Context, Input, Token);";
  private const string APPLY_TRAILER = "return Apply(Context, Input, Token);";

  private readonly ScriptRunner<Task<bool>> _doesApply;
  private readonly ScriptRunner<Task> _apply;

  public ScriptedRule(
    string name,
    string script,
    string[] dependsOn = null,
    string[] provides = null,
    ScriptOptions options = default
  )
  {
    Name = name;
    Provides = provides?.Append(Name).ToArray() ?? new string[] { Name };
    Dependencies = dependsOn ?? new string[] { };
    options ??= GetDefaultOptions<T>();
    var baseCode = Create<bool>(script.FilterScript(),
                                options,
                                globalsType: CONTEXT_TYPE);
    _doesApply = baseCode.ContinueWith<Task<bool>>(DOES_APPLY_TRAILER)
                         .CreateDelegate();
    _apply = baseCode.ContinueWith<Task>(APPLY_TRAILER)
                     .CreateDelegate();
  }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public string Name { get; }

  public async Task Apply(IEngineContext context, T input, CancellationToken t)
      => await await _apply(new ScriptedRuleContext<T>(context, input, t));

  public async Task<bool> DoesApply(IEngineContext context, T input, CancellationToken t)
      => await await _doesApply(new ScriptedRuleContext<T>(context, input, t));
}
