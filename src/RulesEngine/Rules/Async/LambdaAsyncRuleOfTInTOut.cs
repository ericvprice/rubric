namespace RulesEngine.Rules.Async;

public class LambdaAsyncRule<TIn, TOut> : IAsyncRule<TIn, TOut>
{
  private readonly Func<IEngineContext, TIn, TOut, CancellationToken, Task> _body;
  private readonly Func<IEngineContext, TIn, TOut, CancellationToken, Task<bool>> _predicate;

  public LambdaAsyncRule(
      string name,
      Func<IEngineContext, TIn, TOut, CancellationToken, Task<bool>> predicate,
      Func<IEngineContext, TIn, TOut, CancellationToken, Task> body,
      IEnumerable<string> dependencies = null,
      IEnumerable<string> provides = null
  )
  {
    Name = name ?? throw new ArgumentNullException(nameof(name));
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    _body = body ?? throw new ArgumentNullException(nameof(body));
    Dependencies = dependencies?.ToArray() ?? new string[0];
    Provides = provides?.ToArray() ?? new string[0];
  }


  public string Name { get; }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token)
      => _body(context, input, output, token);

  public Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token)
      => _predicate(context, input, output, token);
}
