using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines;
using Rubric.Rules;
using Rubric.Rulesets;

namespace Rubric.Builder;

internal class EngineBuilder<T> : IEngineBuilder<T>
    where T : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal IRuleset<T> Ruleset { get; } = new Ruleset<T>();

  internal ILogger Logger { get; }

  internal IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  public IRuleBuilder<T> WithRule(string name)
      => new RuleBuilder<T>(this, name);

  public IEngineBuilder<T> WithRule(IRule<T> rule)
  {
    Ruleset.AddRule(rule);
    return this;
  }

  public IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules)
  {
    Ruleset.AddRules(rules);
    return this;
  }

  public IEngineBuilder<T> WithExceptionHandler(IExceptionHandler h)
  {
    ExceptionHandler = h;
    return this;
  }

  public IRuleEngine<T> Build() => new RuleEngine<T>(Ruleset, ExceptionHandler, Logger);
}
