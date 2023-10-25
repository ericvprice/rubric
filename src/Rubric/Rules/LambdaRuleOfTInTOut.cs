using static System.String;

namespace Rubric.Rules;

/// <summary>
///   A runtime-constructed processing rule.
/// </summary>
/// <typeparam name="TIn">The engine input type.</typeparam>
/// <typeparam name="TOut">The engine output type.</typeparam>
public class LambdaRule<TIn, TOut> : IRule<TIn, TOut>
{
  private readonly Action<IEngineContext, TIn, TOut> _action;

  private readonly Func<IEngineContext, TIn, TOut, bool> _predicate;

  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="name">Then name for this rule.</param>
  /// <param name="predicate">The predicate.</param>
  /// <param name="action">The action.</param>
  /// <param name="dependencies">A list of dependencies to run before this rule.</param>
  /// <param name="provides">A list of dependencies provided.</param>
  /// <param name="cacheBehavior">The predicate cacheBehavior behavior desired.</param>
  /// <exception cref="ArgumentException">Name is null or empty.</exception>
  /// <exception cref="ArgumentNullException">Predicate or action is null.</exception>
  public LambdaRule(
    string name,
    Func<IEngineContext, TIn, TOut, bool> predicate,
    Action<IEngineContext, TIn, TOut> action,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = IsNullOrWhiteSpace(name)
      ? throw new ArgumentException("Name is required and must be nonempty.", nameof(name))
      : name;
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    _action = action ?? throw new ArgumentNullException(nameof(action));
    Dependencies = dependencies?.ToArray() ?? Array.Empty<string>();
    Provides = provides?.ToArray() ?? Array.Empty<string>();
    CacheBehavior = cacheBehavior;
  }

  /// <inheritdoc />
  public string Name { get; }

  /// <inheritdoc />
  public IEnumerable<string> Dependencies { get; }

  /// <inheritdoc />
  public IEnumerable<string> Provides { get; }

  /// <inheritdoc />
  public PredicateCaching CacheBehavior { get; }

  /// <inheritdoc />
  public void Apply(IEngineContext context, TIn input, TOut output)
    => _action(context, input, output);

  /// <inheritdoc />
  public bool DoesApply(IEngineContext context, TIn input, TOut output)
    => _predicate(context, input, output);
}