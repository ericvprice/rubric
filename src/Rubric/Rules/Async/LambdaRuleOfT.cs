namespace Rubric.Rules.Async;

/// <inheritdoc />
public class LambdaRule<T> : IRule<T>
{
  private readonly Func<IEngineContext, T, CancellationToken, Task> _action;

  private readonly Func<IEngineContext, T, CancellationToken, Task<bool>> _predicate = (_, _, _)
    => Task.FromResult(true);
  
  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="name">Then name for this rule.</param>
  /// <param name="predicate">The predicate.</param>
  /// <param name="action">The action.</param>
  /// <param name="dependencies">A list of dependencies to run before this rule.</param>
  /// <param name="provides">A list of dependencies provided.</param>
  /// <param name="cacheBehavior">The predicate caching behavior desired.</param>
  /// <exception cref="ArgumentException">Name is null or empty.</exception>
  /// <exception cref="ArgumentNullException">Predicate or action is null.</exception>
  public LambdaRule(
    string name,
    Func<IEngineContext, T, CancellationToken, Task<bool>> predicate,
    Func<IEngineContext, T, CancellationToken, Task> action,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = string.IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
    _action = action ?? throw new ArgumentNullException(nameof(action));
    _predicate = predicate ?? _predicate;
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
  public Task Apply(IEngineContext context, T input, CancellationToken token)
    => _action(context, input, token);

  /// <inheritdoc />
  public Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
    => _predicate(context, input, token);
}