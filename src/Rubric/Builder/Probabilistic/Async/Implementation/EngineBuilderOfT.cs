using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Probabilistic.Async;
using Rubric.Engines.Probabilistic.Async.Implementation;
using Rubric.Rules.Probabilistic.Async;
using Rubric.Rulesets.Probabilistic.Async;

namespace Rubric.Builder.Probabilistic.Async.Implementation;

internal class EngineBuilder<T> : IEngineBuilder<T>
  where T : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal IRuleset<T> AsyncRuleset { get; } = new Ruleset<T>();

  public ILogger Logger { get; }

  public bool IsParallel { get; private set; }

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  /// <inheritdoc />
  public IRuleBuilder<T> WithRule(string name)
    => new RuleBuilder<T>(this, name);

  /// <inheritdoc />
  public IEngineBuilder<T> WithRule(IRule<T> rule)
  {
    AsyncRuleset.AddRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRule(Rules.Probabilistic.IRule<T> rule)
  {
    AsyncRuleset.AddRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRules(IEnumerable<Rules.Probabilistic.IRule<T>> rules)
  {
    AsyncRuleset.AddRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules)
  {
    AsyncRuleset.AddRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> AsParallel()
  {
    IsParallel = true;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler)
  {
    ExceptionHandler = handler;
    return this;
  }

  /// <inheritdoc />
  public IRuleEngine<T> Build()
    => new RuleEngine<T>(AsyncRuleset, IsParallel, ExceptionHandler, Logger);
}