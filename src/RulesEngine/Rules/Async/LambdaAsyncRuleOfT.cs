namespace RulesEngine.Rules.Async;

public class LambdaAsyncRule<T> : IAsyncRule<T>
{
  private readonly Func<IEngineContext, T, CancellationToken, Task> _body;
  private readonly Func<IEngineContext, T, CancellationToken, Task<bool>> _predicate;

  public LambdaAsyncRule(
      string name,
      Func<IEngineContext, T, CancellationToken, Task<bool>> predicate,
      Func<IEngineContext, T, CancellationToken, Task> body,
      IEnumerable<string> dependencies = null,
      IEnumerable<string> provides = null
  )
  {
    Name = string.IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    _body = body ?? throw new ArgumentNullException(nameof(body));
    Dependencies = dependencies?.ToArray() ?? new string[0];
    Provides = provides?.ToArray() ?? new string[0];
  }


  public string Name { get; }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public Task Apply(IEngineContext context, T input, CancellationToken token)
      => _body(context, input, token);

  public Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
      => _predicate(context, input, token);
}
