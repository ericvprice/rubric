using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines;
using Rubric.Rules;
using Rubric.Rules.Async;
using Rubric.Rulesets.Async;

namespace Rubric.Builder;

internal class AsyncEngineBuilder<T> : IAsyncEngineBuilder<T>
  where T : class
{
  public AsyncEngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  public ILogger Logger { get; }

  public bool IsParallel { get; private set; }

  internal AsyncRuleset<T> AsyncRuleset { get; } = new();

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  public IAsyncRuleEngine<T> Build()
    => new AsyncRuleEngine<T>(AsyncRuleset, IsParallel, ExceptionHandler, Logger);

  public IAsyncRuleBuilder<T> WithRule(string name)
    => new AsyncRuleBuilder<T>(this, name);

  public IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule)
  {
    AsyncRuleset.AddAsyncRule(rule);
    return this;
  }

  public IAsyncEngineBuilder<T> WithRule(IRule<T> rule)
  {
    AsyncRuleset.AddAsyncRule(rule.WrapAsync());
    return this;
  }

  public IAsyncEngineBuilder<T> WithRules(IEnumerable<IAsyncRule<T>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules);
    return this;
  }

  public IAsyncEngineBuilder<T> AsParallel()
  {
    IsParallel = true;
    return this;
  }

  public IAsyncEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler)
  {
    ExceptionHandler = handler;
    return this;
  }
}