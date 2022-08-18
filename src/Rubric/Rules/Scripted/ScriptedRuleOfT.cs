using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rules.Async;
using static Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;
using static Rubric.Rules.Scripted.ScriptingHelpers;

namespace Rubric.Rules.Scripted;

public class ScriptedRule<T> : IAsyncRule<T>
{
  private const string DOES_APPLY_TRAILER = "return DoesApply(Context, Input, Token);";
  private const string APPLY_TRAILER = "return Apply(Context, Input, Token);";
  private static readonly Type _contextType = typeof(ScriptedRuleContext<T>);
  private readonly ScriptRunner<Task> _apply;

  private readonly ScriptRunner<Task<bool>> _doesApply;

  public ScriptedRule(
    string name,
    string script,
    ScriptOptions options = null,
    string[] dependsOn = null,
    string[] provides = null
  )
  {
    Name = name;
    Provides = provides?.Append(Name).ToArray() ?? new[] { Name };
    Dependencies = dependsOn ?? new string[] { };
    options ??= GetDefaultOptions<T>();
    var baseCode = Create<bool>(script.FilterScript(),
                                options,
                                _contextType);
    _doesApply = baseCode.ContinueWith<Task<bool>>(DOES_APPLY_TRAILER)
                         .CreateDelegate();
    _apply = baseCode.ContinueWith<Task>(APPLY_TRAILER)
                     .CreateDelegate();
  }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public string Name { get; }

  public async Task Apply(IEngineContext context, T input, CancellationToken t)
    => await await _apply(new ScriptedRuleContext<T>(context, input, t), t);

  public async Task<bool> DoesApply(IEngineContext context, T input, CancellationToken t)
    => await await _doesApply(new ScriptedRuleContext<T>(context, input, t), t);
}