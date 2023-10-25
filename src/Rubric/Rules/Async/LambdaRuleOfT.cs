namespace Rubric.Rules.Async;

public class LambdaRule<T> : IRule<T>
{
  private readonly Func<IEngineContext, T, CancellationToken, Task> _body;

  private readonly Func<IEngineContext, T, CancellationToken, Task<bool>> _predicate = (_, _, _)
    => Task.FromResult(true);

  public LambdaRule(
    string name,
    Func<IEngineContext, T, CancellationToken, Task<bool>> predicate,
    Func<IEngineContext, T, CancellationToken, Task> body,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = string.IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
    _body = body ?? throw new ArgumentNullException(nameof(body));
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
    => _body(context, input, token);

  /// <inheritdoc />
  public Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
    => _predicate(context, input, token);
}