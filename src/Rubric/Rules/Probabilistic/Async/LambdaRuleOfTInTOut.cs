namespace Rubric.Rules.Probabilistic.Async;

public class LambdaRule<TIn, TOut> : IRule<TIn, TOut>
{
  private readonly Func<IEngineContext, TIn, TOut, CancellationToken, Task> _body;

  private readonly Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> _predicate = (_, _, _, _)
    => Task.FromResult(1D);

  public LambdaRule(
    string name,
    Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> predicate,
    Func<IEngineContext, TIn, TOut, CancellationToken, Task> body,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null,
    PredicateCaching cacheBehavior = default
  )
  {
    Name = name ?? throw new ArgumentNullException(nameof(name));
    _body = body ?? throw new ArgumentNullException(nameof(body));
    _predicate = predicate ?? _predicate;
    Dependencies = dependencies?.ToArray() ?? Array.Empty<string>();
    Provides = provides?.ToArray() ?? Array.Empty<string>();
    CacheBehavior = cacheBehavior;
  }


  /// <inheritdoc/>
  public string Name { get; }

  /// <inheritdoc/>
  public IEnumerable<string> Dependencies { get; }

  /// <inheritdoc/>
  public IEnumerable<string> Provides { get; }
  
  /// <inheritdoc />
  public PredicateCaching CacheBehavior { get; }


  /// <inheritdoc/>
  public Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => _body(context, input, output, token);

  /// <inheritdoc/>
  public Task<double> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
    => _predicate(context, input, output, token);
}