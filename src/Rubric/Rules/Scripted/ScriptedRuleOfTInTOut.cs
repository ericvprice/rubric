using Microsoft.CodeAnalysis.Scripting;
using static Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;
using static Rubric.Rules.Scripted.ScriptingHelpers;

namespace Rubric.Rules.Scripted;

/// <summary>
///   Represent a dynamically compiled rule.
/// </summary>
/// <typeparam name="TIn">The rule input type.</typeparam>
/// <typeparam name="TOut">The rule output type.</typeparam>
public class ScriptedRule<TIn, TOut> : Async.IRule<TIn, TOut>
{
  private const string DoesApplyTrailer = "return DoesApply(Context, Input, Output, Token);";
  private const string ApplyTrailer = "return Apply(Context, Input, Output, Token);";

  private static readonly Type _contextType = typeof(ScriptedRuleContext<TIn, TOut>);
  private readonly ScriptRunner<Task> _apply;

  private readonly ScriptRunner<Task<bool>> _doesApply;

  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="name">The name for this rule.</param>
  /// <param name="script">The C# script to compile.</param>
  /// <param name="options">The script compilation options.</param>
  /// <param name="dependsOn">The list of required dependencies.</param>
  /// <param name="provides">The list of provided dependencies.</param>
  /// <param name="cacheBehavior">The desired predicate caching behavior.</param>
  public ScriptedRule(
    string name,
    string script,
    ScriptOptions options = null,
    string[] dependsOn = null,
    string[] provides = null,
    PredicateCaching cacheBehavior = default)
  {
    Dependencies = dependsOn ?? Array.Empty<string>();
    Provides = provides ?? Array.Empty<string>();
    Name = name;
    options ??= GetDefaultOptions<TIn, TOut>();
    var baseScript = Create<bool>(script.FilterScript(),
                                  options,
                                  _contextType);
    _doesApply = baseScript.ContinueWith<Task<bool>>(DoesApplyTrailer)
                           .CreateDelegate();
    _apply = baseScript.ContinueWith<Task>(ApplyTrailer)
                       .CreateDelegate();
    CacheBehavior = cacheBehavior;
  }

  /// <inheritdoc />
  public IEnumerable<string> Dependencies { get; }

  /// <inheritdoc />
  public IEnumerable<string> Provides { get; }

  /// <inheritdoc />
  public string Name { get; }

  /// <inheritdoc />
  public PredicateCaching CacheBehavior { get; }

  /// <inheritdoc />
  public async Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => await (await _apply(new ScriptedRuleContext<TIn, TOut>(context, input, output, token), token)
      .ConfigureAwait(false)).ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => await (await _doesApply(new ScriptedRuleContext<TIn, TOut>(context, input, output, token), token)
      .ConfigureAwait(false)).ConfigureAwait(false);
}