namespace Rubric.Rules.Probabilistic.Async;

/// <inheritdoc />
public class LambdaRule<TIn, TOut> : IRule<TIn, TOut>
{
  private readonly Func<IEngineContext, TIn, TOut, CancellationToken, Task> _action;

  private readonly Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> _predicate = (_, _, _, _)
    => Task.FromResult(1D);

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
    Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> predicate,
    Func<IEngineContext, TIn, TOut, CancellationToken, Task> action,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = name ?? throw new ArgumentNullException(nameof(name));
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
  public Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => _action(context, input, output, token);

  /// <inheritdoc />
  public Task<double> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => _predicate(context, input, output, token);
}