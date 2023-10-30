using Microsoft.CodeAnalysis.Scripting;
using static Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript;
using static Rubric.Rules.Scripted.ScriptingHelpers;

namespace Rubric.Rules.Scripted;

/// <summary>
///   A dynamically compiled rule.
/// </summary>
/// <typeparam name="T">The rule type.</typeparam>
public class ScriptedRule<T> : Async.IRule<T>
{
  private const string DoesApplyTrailer = "return DoesApply(Context, Input, Token);";
  private const string ApplyTrailer = "return Apply(Context, Input, Token);";
  private static readonly Type _contextType = typeof(ScriptedRuleContext<T>);
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
    IEnumerable<string> dependsOn = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = name;
    Provides = provides?.Append(Name).ToArray() ?? new[] { Name };
    Dependencies = dependsOn?.ToArray() ?? Array.Empty<string>();
    options ??= GetDefaultOptions<T>();
    var baseCode = Create<bool>(script.FilterScript(),
                                options,
                                _contextType);
    _doesApply = baseCode.ContinueWith<Task<bool>>(DoesApplyTrailer)
                         .CreateDelegate();
    _apply = baseCode.ContinueWith<Task>(ApplyTrailer)
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
  public async Task Apply(IEngineContext context, T input, CancellationToken token)
    => await (await _apply(new ScriptedRuleContext<T>(context, input, token), token).ConfigureAwait(false))
      .ConfigureAwait(false);

  /// <inheritdoc />
  public async Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
    => await (await _doesApply(new ScriptedRuleContext<T>(context, input, token), token).ConfigureAwait(false))
      .ConfigureAwait(false);
}