using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Builder;

internal class AsyncEngineBuilder<TIn, TOut> : IAsyncEngineBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  public AsyncEngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  public ILogger Logger { get; }

  public bool IsParallel { get; private set; }

  public IExceptionHandler ExceptionHandler { get; private set; }

  internal AsyncRuleset<TIn, TOut> AsyncRuleset { get; } = new();

  public IAsyncRuleEngine<TIn, TOut> Build()
    => new AsyncRuleEngine<TIn, TOut>(AsyncRuleset, IsParallel, ExceptionHandler, Logger);

  public IAsyncPostRuleBuilder<TIn, TOut> WithPostRule(string name)
    => new AsyncPostRuleBuilder<TIn, TOut>(this, name);

  public IAsyncEngineBuilder<TIn, TOut> WithPostRule(IAsyncRule<TOut> rule)
  {
    AsyncRuleset.AddAsyncPostRule(rule);
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> WithPreRule(string name)
    => new AsyncPreRuleBuilder<TIn, TOut>(this, name);

  public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IAsyncRule<TIn> rule)
  {
    AsyncRuleset.AddAsyncPreRule(rule);
    return this;
  }

  public IAsyncRuleBuilder<TIn, TOut> WithRule(string name)
    => new AsyncRuleBuilder<TIn, TOut>(this, name);

  public IAsyncEngineBuilder<TIn, TOut> WithRule(IAsyncRule<TIn, TOut> rule)
  {
    AsyncRuleset.AddAsyncRule(rule);
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule)
  {
    AsyncRuleset.AddAsyncPostRule(rule.WrapAsync());
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule)
  {
    AsyncRuleset.AddAsyncPreRule(rule.WrapAsync());
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
  {
    AsyncRuleset.AddAsyncRule(rule.WrapAsync());
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> AsParallel()
  {
    IsParallel = true;
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler)
  {
    ExceptionHandler = handler;
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules)
  {
    AsyncRuleset.AddAsyncPreRules(rules);
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules);
    return this;
  }

  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules)
  {
    AsyncRuleset.AddAsyncPostRules(rules);
    return this;
  }
}