using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Probabilistic.Async;
using Rubric.Engines.Probabilistic.Async.Implementation;
using Rubric.Rules.Probabilistic;
using Rubric.Rulesets.Probabilistic.Async;

namespace Rubric.Builder.Probabilistic.Async.Implementation;

/// <inheritdoc />
internal class EngineBuilder<TIn, TOut> : IEngineBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal IRuleset<TIn, TOut> AsyncRuleset { get; } = new Ruleset<TIn, TOut>();

  public ILogger Logger { get; }

  public bool IsParallel { get; private set; }

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  public IPreRuleBuilder<TIn, TOut> WithPreRule(string name)
    => new PreRuleBuilder<TIn, TOut>(this, name);

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule)
  {
    AsyncRuleset.AddPreRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPreRule(Rules.Probabilistic.Async.IRule<TIn> rule)
  {
    AsyncRuleset.AddPreRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rules)
  {
    AsyncRuleset.AddPreRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<Rules.Probabilistic.Async.IRule<TIn>> rules)
  {
    AsyncRuleset.AddPreRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithRule(string name)
    => new RuleBuilder<TIn, TOut>(this, name);

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithRule(Rules.Probabilistic.Async.IRule<TIn, TOut> rule)
  {
    AsyncRuleset.AddRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
  {
    AsyncRuleset.AddRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rules)
  {
    AsyncRuleset.AddRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithRules(IEnumerable<Rules.Probabilistic.Async.IRule<TIn, TOut>> rules)
  {
    AsyncRuleset.AddRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithPostRule(string name)
    => new PostRuleBuilder<TIn, TOut>(this, name);

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule)
  {
    AsyncRuleset.AddPostRule(rule.WrapAsync());
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPostRule(Rules.Probabilistic.Async.IRule<TOut> rule)
  {
    AsyncRuleset.AddPostRule(rule);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rules)
  {
    AsyncRuleset.AddPostRules(rules.Select(r => r.WrapAsync()));
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<Rules.Probabilistic.Async.IRule<TOut>> rules)
  {
    AsyncRuleset.AddPostRules(rules);
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> AsParallel()
  {
    IsParallel = true;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler)
  {
    ExceptionHandler = handler;
    return this;
  }

  public IRuleEngine<TIn, TOut> Build()
    => new RuleEngine<TIn, TOut>(AsyncRuleset, IsParallel, ExceptionHandler, Logger);
}