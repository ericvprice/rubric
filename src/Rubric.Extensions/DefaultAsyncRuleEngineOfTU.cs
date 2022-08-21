using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Rules.Async;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions;

[ExcludeFromCodeCoverage]
internal class DefaultAsyncRuleEngine<TIn, TOut> : IAsyncRuleEngine<TIn, TOut> where TIn : class where TOut : class
{
  private readonly IAsyncRuleEngine<TIn, TOut> _instance;

  public DefaultAsyncRuleEngine(
    IAsyncEngineBuilder<TIn, TOut> builder,
    IEnumerable<IAsyncRule<TIn>> preRules,
    IEnumerable<IAsyncRule<TIn, TOut>> rules,
    IEnumerable<IAsyncRule<TOut>> postRules)
  => _instance = builder.WithAsyncPreRules(preRules)
                        .WithAsyncRules(rules)
                        .WithAsyncPostRules(postRules)
                        .Build();

  public ILogger Logger => _instance.Logger;

  public bool IsAsync => _instance.IsAsync;

  public Type InputType => _instance.InputType;

  public Type OutputType => _instance.OutputType;

  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  public IEnumerable<IAsyncRule<TIn>> PreRules => _instance.PreRules;

  public IEnumerable<IAsyncRule<TOut>> PostRules => _instance.PostRules;

  public IEnumerable<IAsyncRule<TIn, TOut>> Rules => _instance.Rules;

  public bool IsParallel => _instance.IsParallel;

  public Task ApplyAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(input, output, context, token);

  public Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default)
     => _instance.ApplyAsync(inputs, output, context, token);

  public Task ApplyAsync(IAsyncEnumerable<TIn> inputStream, TOut output, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(inputStream, output, context, token);

  public Task ApplyParallelAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyParallelAsync(inputs, output, context, token);
}