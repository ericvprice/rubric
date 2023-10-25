using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Async;
using Rubric.Engines.Async.Implementation;
using Rubric.Rules.Async;
using Rubric.Rulesets.Async;

namespace Rubric.Builder.Async.Implementation;

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
    AsyncRuleset.AddAsyncRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRule(Rules.IRule<T> rule)
  {
    AsyncRuleset.AddAsyncRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRules(IEnumerable<Rules.IRule<T>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules);
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