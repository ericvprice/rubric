using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Async;
using Rubric.Engines.Async;
using Rubric.Rules;
using Rubric.Rules.Async;
using Rubric.Rulesets.Async;

namespace Rubric.Builder.Async;

internal class AsyncEngineBuilder<T> : IAsyncEngineBuilder<T>
  where T : class
{
  internal AsyncEngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal ILogger Logger { get; }

  internal bool IsParallel { get; private set; }

  internal IAsyncRuleset<T> AsyncRuleset { get; } = new AsyncRuleset<T>();

  internal IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> WithRule(string name)
    => new AsyncRuleBuilder<T>(this, name);

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule)
  {
    AsyncRuleset.AddAsyncRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> WithRule(IRule<T> rule)
  {
    AsyncRuleset.AddAsyncRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> WithRules(IEnumerable<IAsyncRule<T>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> AsParallel()
  {
    IsParallel = true;
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler)
  {
    ExceptionHandler = handler;
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleEngine<T> Build()
    => new AsyncRuleEngine<T>(AsyncRuleset, IsParallel, ExceptionHandler, Logger);

}