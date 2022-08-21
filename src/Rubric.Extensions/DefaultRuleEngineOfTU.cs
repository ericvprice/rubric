using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Rules;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions;

[ExcludeFromCodeCoverage]
internal class DefaultRuleEngine<TIn, TOut> : IRuleEngine<TIn, TOut> where TIn : class where TOut : class
{
  private readonly IRuleEngine<TIn, TOut> _instance;

  public DefaultRuleEngine(
    IEngineBuilder<TIn, TOut> builder,
    IEnumerable<IRule<TIn>> preRules,
    IEnumerable<IRule<TIn, TOut>> rules,
    IEnumerable<IRule<TOut>> postRules
  ) => _instance = builder.WithPreRules(preRules)
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

  public void Apply(TIn input, TOut output, IEngineContext context = null)
    => _instance.Apply(input, output, context);

  public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
    => _instance.Apply(inputs, output, context);
}