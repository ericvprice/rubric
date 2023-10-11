using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Probabilistic.Async;
using Rubric.Rules.Probabilistic.Async;
using Rubric.Rulesets.Probabilistic.Async;

namespace Rubric.Probabilistic.Async.Builder.Default;

internal class EngineBuilder<TIn, TOut> : IEngineBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
    internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

    internal ILogger Logger { get; }

    internal bool IsParallel { get; private set; }

    internal IExceptionHandler ExceptionHandler { get; private set; }

    internal IRuleset<TIn, TOut> AsyncRuleset { get; } = new Ruleset<TIn, TOut>();

    public IPreRuleBuilder<TIn, TOut> WithAsyncPreRule(string name)
      => new PreRuleBuilder<TIn, TOut>(this, name);

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithPreRule(Rules.Probabilistic.IRule<TIn> rule)
    {
        AsyncRuleset.AddPreRule(rule.WrapAsync());
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPreRule(IRule<TIn> rule)
    {
        AsyncRuleset.AddPreRule(rule);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<Rules.Probabilistic.IRule<TIn>> rules)
    {
        AsyncRuleset.AddPreRules(rules.Select(r => r.WrapAsync()));
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<IRule<TIn>> rules)
    {
        AsyncRuleset.AddPreRules(rules);
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> WithAsyncRule(string name)
      => new RuleBuilder<TIn, TOut>(this, name);

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncRule(IRule<TIn, TOut> rule)
    {
        AsyncRuleset.AddRule(rule);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithRule(Rules.Probabilistic.IRule<TIn, TOut> rule)
    {
        AsyncRuleset.AddRule(rule.WrapAsync());
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithRules(IEnumerable<Rules.Probabilistic.IRule<TIn, TOut>> rules)
    {
        AsyncRuleset.AddRules(rules.Select(r => r.WrapAsync()));
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<IRule<TIn, TOut>> rules)
    {
        AsyncRuleset.AddRules(rules);
        return this;
    }

    /// <inheritdoc />
    public IPostRuleBuilder<TIn, TOut> WithAsyncPostRule(string name)
      => new PostRuleBuilder<TIn, TOut>(this, name);

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPostRule(Rules.Probabilistic.IRule<TOut> rule)
    {
        AsyncRuleset.AddPostRule(rule.WrapAsync());
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPostRule(IRule<TOut> rule)
    {
        AsyncRuleset.AddPostRule(rule);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<Rules.Probabilistic.IRule<TOut>> rules)
    {
        AsyncRuleset.AddPostRules(rules.Select(r => r.WrapAsync()));
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<IRule<TOut>> rules)
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
  public IEngineBuilder<TIn, TOut> WithPreRule(Rules.IRule<TIn> rule) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncPreRule(Rules.Async.IRule<TIn> rule) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<Rules.IRule<TIn>> rules) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<Rules.Async.IRule<TIn>> rules) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncRule(Rules.Async.IRule<TIn, TOut> rule) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithRule(Rules.IRule<TIn, TOut> rule) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithRules(IEnumerable<Rules.IRule<TIn, TOut>> rules) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<Rules.Async.IRule<TIn, TOut>> rules) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncPostRule(Rules.IRule<TOut> rule) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncPostRule(Rules.Async.IRule<TOut> rule) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<Rules.IRule<TOut>> rules) => throw new NotImplementedException();
  public IEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<Rules.Async.IRule<TOut>> rules) => throw new NotImplementedException();
}