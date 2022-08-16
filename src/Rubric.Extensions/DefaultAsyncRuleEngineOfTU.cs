using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Rules.Async;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions;

[ExcludeFromCodeCoverage]
internal class DefaultAsyncRuleEngine<T, U> : IAsyncRuleEngine<T, U> where T : class where U : class
{
  private readonly IAsyncRuleEngine<T, U> _instance;

  public DefaultAsyncRuleEngine(
    IAsyncEngineBuilder<T, U> builder,
    IEnumerable<IAsyncRule<T>> preRules,
    IEnumerable<IAsyncRule<T, U>> rules,
    IEnumerable<IAsyncRule<U>> postRules)
  => _instance = builder.WithAsyncPreRules(preRules)
                        .WithAsyncRules(rules)
                        .WithAsyncPostRules(postRules)
                        .Build();

  public ILogger Logger => _instance.Logger;

  public bool IsAsync => _instance.IsAsync;

  public Type InputType => _instance.InputType;

  public Type OutputType => _instance.OutputType;

  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  public EngineException LastException => _instance.LastException;

  public IEnumerable<IAsyncRule<T>> PreRules => _instance.PreRules;

  public IEnumerable<IAsyncRule<U>> PostRules => _instance.PostRules;

  public IEnumerable<IAsyncRule<T, U>> Rules => _instance.Rules;

  public bool IsParallel => _instance.IsParallel;

  public Task ApplyAsync(T input, U output, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(input, output, context, token);

  public Task ApplyAsync(IEnumerable<T> inputs, U output, IEngineContext context = null, CancellationToken token = default)
     => _instance.ApplyAsync(inputs, output, context, token);

  public Task ApplyAsync(IAsyncEnumerable<T> inputStream, U output, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(inputStream, output, context, token);

  public Task ApplyParallelAsync(IEnumerable<T> inputs, U output, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyParallelAsync(inputs, output, context, token);
}