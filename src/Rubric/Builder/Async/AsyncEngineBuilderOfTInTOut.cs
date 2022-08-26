using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Async;
using Rubric.Engines.Async;
using Rubric.Rules;
using Rubric.Rules.Async;
using Rubric.Rulesets.Async;

namespace Rubric.Builder.Async;

internal class AsyncEngineBuilder<TIn, TOut> : IAsyncEngineBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  internal AsyncEngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal ILogger Logger { get; }

  internal bool IsParallel { get; private set; }

  internal IExceptionHandler ExceptionHandler { get; private set; }

  internal IAsyncRuleset<TIn, TOut> AsyncRuleset { get; } = new AsyncRuleset<TIn, TOut>();

  public IAsyncPreRuleBuilder<TIn, TOut> WithAsyncPreRule(string name)
    => new AsyncPreRuleBuilder<TIn, TOut>(this, name);

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule)
  {
    AsyncRuleset.AddAsyncPreRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPreRule(IAsyncRule<TIn> rule)
  {
    AsyncRuleset.AddAsyncPreRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rules)
  {
    AsyncRuleset.AddAsyncPreRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules)
  {
    AsyncRuleset.AddAsyncPreRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<TIn, TOut> WithAsyncRule(string name)
    => new AsyncRuleBuilder<TIn, TOut>(this, name);

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncRule(IAsyncRule<TIn, TOut> rule)
  {
    AsyncRuleset.AddAsyncRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
  {
    AsyncRuleset.AddAsyncRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules)
  {
    AsyncRuleset.AddAsyncRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IAsyncPostRuleBuilder<TIn, TOut> WithAsyncPostRule(string name)
    => new AsyncPostRuleBuilder<TIn, TOut>(this, name);

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRule(IRule<TOut> rule)
  {
    AsyncRuleset.AddAsyncPostRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRule(IAsyncRule<TOut> rule)
  {
    AsyncRuleset.AddAsyncPostRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rules)
  {
    AsyncRuleset.AddAsyncPostRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules)
  {
    AsyncRuleset.AddAsyncPostRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> AsParallel()
  {
    IsParallel = true;
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler)
  {
    ExceptionHandler = handler;
    return this;
  }

  public IAsyncRuleEngine<TIn, TOut> Build()
    => new AsyncRuleEngine<TIn, TOut>(AsyncRuleset, IsParallel, ExceptionHandler, Logger);
}