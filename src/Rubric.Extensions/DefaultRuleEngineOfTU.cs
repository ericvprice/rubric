using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Rules;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions;

[ExcludeFromCodeCoverage]
internal class DefaultRuleEngine<T, U> : IRuleEngine<T, U> where T : class where U : class
{
  private readonly IRuleEngine<T, U> _instance;

  public DefaultRuleEngine(
    IEngineBuilder<T, U> builder,
    IEnumerable<IRule<T>> preRules,
    IEnumerable<IRule<T, U>> rules,
    IEnumerable<IRule<U>> postRules
  ) => _instance = builder.WithPreRules(preRules)
                          .WithRules(rules)
                          .WithPostRules(postRules)
                          .Build();

  public ILogger Logger => _instance.Logger;

  public bool IsAsync => _instance.IsAsync;

  public Type InputType => _instance.InputType;

  public Type OutputType => _instance.OutputType;

  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  public EngineException LastException { get => _instance.LastException; set => _instance.LastException = value; }

  public IEnumerable<IRule<T>> PreRules => _instance.PreRules;

  public IEnumerable<IRule<U>> PostRules => _instance.PostRules;

  public IEnumerable<IRule<T, U>> Rules => _instance.Rules;

  public void Apply(T input, U output, IEngineContext context = null)
    => _instance.Apply(input, output, context);

  public void Apply(IEnumerable<T> inputs, U output, IEngineContext context = null)
    => _instance.Apply(inputs, output, context);
}