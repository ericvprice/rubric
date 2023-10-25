using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines;
using Rubric.Engines.Implementation;
using Rubric.Rules;
using Rubric.Rulesets;

namespace Rubric.Builder.Implementation;

internal class EngineBuilder<T> : IEngineBuilder<T>
  where T : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal IRuleset<T> Ruleset { get; } = new Ruleset<T>();

  public ILogger Logger { get; }

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  /// <inheritdoc />
  public IRuleBuilder<T> WithRule(string name)
    => new RuleBuilder<T>(this, name);

  /// <inheritdoc />
  public IEngineBuilder<T> WithRule(IRule<T> rule)
  {
    Ruleset.AddRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules)
  {
    Ruleset.AddRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithExceptionHandler(IExceptionHandler h)
  {
    ExceptionHandler = h;
    return this;
  }

  /// <inheritdoc />
  public IRuleEngine<T> Build() => new RuleEngine<T>(Ruleset, ExceptionHandler, Logger);
}