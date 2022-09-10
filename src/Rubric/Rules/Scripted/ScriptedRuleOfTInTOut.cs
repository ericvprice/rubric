using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rules.Async;
using static Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;
using static Rubric.Rules.Scripted.ScriptingHelpers;

namespace Rubric.Rules.Scripted;

public class ScriptedRule<TIn, TOut> : IAsyncRule<TIn, TOut>
{
  private const string DOES_APPLY_TRAILER = "return DoesApply(Context, Input, Output, Token);";
  private const string APPLY_TRAILER = "return Apply(Context, Input, Output, Token);";

  private static readonly Type _contextType = typeof(ScriptedRuleContext<TIn, TOut>);
  private readonly ScriptRunner<Task> _apply;

  private readonly ScriptRunner<Task<bool>> _doesApply;

  public ScriptedRule(
    string name,
    string script,
    ScriptOptions options = null,
    string[] dependsOn = null,
    string[] provides = null)
  {
    Dependencies = dependsOn ?? new string[] { };
    Provides = provides ?? new string[] { };
    Name = name;
    options ??= GetDefaultOptions<TIn, TOut>();
    var baseScript = Create<bool>(script.FilterScript(),
                                  options,
                                  _contextType);
    _doesApply = baseScript.ContinueWith<Task<bool>>(DOES_APPLY_TRAILER)
                           .CreateDelegate();
    _apply = baseScript.ContinueWith<Task>(APPLY_TRAILER)
                       .CreateDelegate();
  }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public string Name { get; }

  public async Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken t)
    => await await _apply(new ScriptedRuleContext<TIn, TOut>(context, input, output, t), t);

  public async Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken t)
    => await await _doesApply(new ScriptedRuleContext<TIn, TOut>(context, input, output, t), t);
}