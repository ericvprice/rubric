using Microsoft.Extensions.Logging;
using Rubric.Rules.Async;
using System.Diagnostics.CodeAnalysis;
using Rubric.Async;
using Rubric.Builder.Async;

namespace Rubric.Extensions;

[ExcludeFromCodeCoverage]
internal class DefaultAsyncRuleEngine<T> : IAsyncRuleEngine<T> where T : class
{
  private readonly IAsyncRuleEngine<T> _instance;

  public DefaultAsyncRuleEngine(IAsyncEngineBuilder<T> builder, IEnumerable<IAsyncRule<T>> rules)
    => _instance = builder.WithRules(rules).Build();

  public IEnumerable<IAsyncRule<T>> Rules => _instance.Rules;

  public ILogger Logger => _instance.Logger;

  public bool IsAsync => _instance.IsAsync;

  public Type InputType => _instance.InputType;

  public Type OutputType => _instance.OutputType;

  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  public bool IsParallel => _instance.IsParallel;

  public Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(input, context, token);

  public Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null, bool parallelizeInputs = false, CancellationToken token = default)
    => _instance.ApplyAsync(inputs, context, parallelizeInputs, token);

  public Task ApplyAsync(IAsyncEnumerable<T> inputStream, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(inputStream, context, token);
}