namespace Rubric.Rules.Probabilistic;

/// <summary>
///   A runtime-constructed processing rule.
/// </summary>
/// <typeparam name="T">The engine input type.</typeparam>
public class LambdaRule<T> : IRule<T>
{
  private readonly Action<IEngineContext, T> _action;

  private readonly Func<IEngineContext, T, double> _predicate;

  public LambdaRule(
    string name,
    Func<IEngineContext, T, double> predicate,
    Action<IEngineContext, T> action,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = string.IsNullOrWhiteSpace(name)
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
  public void Apply(IEngineContext context, T input)
    => _action(context, input);

  /// <inheritdoc />
  public double DoesApply(IEngineContext context, T input)
    => _predicate(context, input);
}