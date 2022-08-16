using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Rules;

namespace Rubric.Extensions;

internal class DefaultRuleEngine<T> : IRuleEngine<T> where T : class
{
  private readonly IRuleEngine<T> _instance;

  public DefaultRuleEngine(IEngineBuilder<T> builder, IEnumerable<IRule<T>> rules)
    => _instance = builder.WithRules(rules).Build();

  public IEnumerable<IRule<T>> Rules => _instance.Rules;

  public ILogger Logger => _instance.Logger;

  public bool IsAsync => _instance.IsAsync;

  public Type InputType => _instance.InputType;

  public Type OutputType => _instance.OutputType;

  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  public EngineException LastException { get => _instance.LastException; set => _instance.LastException = value; }

  public void Apply(T input, IEngineContext context = null)
    => _instance.Apply(input, context);

  public void Apply(IEnumerable<T> inputs, IEngineContext context = null)
    => _instance.Apply(inputs, context);
}