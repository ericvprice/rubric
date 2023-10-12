using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Rubric.Rules.Async;
using Rubric.Engines.Async;
using Rubric.Builder.Async;

namespace Rubric.Extensions;

[ExcludeFromCodeCoverage]
internal class DefaultAsyncRuleEngine<TIn, TOut> : IRuleEngine<TIn, TOut> where TIn : class where TOut : class
{
  private readonly IRuleEngine<TIn, TOut> _instance;

  public DefaultAsyncRuleEngine(
    IEngineBuilder<TIn, TOut> builder,
    IEnumerable<IRule<TIn>> preRules,
    IEnumerable<IRule<TIn, TOut>> rules,
    IEnumerable<IRule<TOut>> postRules)
  => _instance = builder.WithPreRules(preRules)
                        .WithRules(rules)
                        .WithPostRules(postRules)
                        .Build();

  public ILogger Logger => _instance.Logger;

  public bool IsAsync => _instance.IsAsync;

  public Type InputType => _instance.InputType;

  public Type OutputType => _instance.OutputType;

  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  public IEnumerable<IRule<TIn>> PreRules => _instance.PreRules;

  public IEnumerable<IRule<TOut>> PostRules => _instance.PostRules;

  public IEnumerable<IRule<TIn, TOut>> Rules => _instance.Rules;

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