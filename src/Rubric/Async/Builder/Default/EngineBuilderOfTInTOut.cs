using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Engines.Async;
using Rubric.Rules.Async;
using Rubric.Rulesets.Async;

namespace Rubric.Async.Builder.Default;

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
    public IEngineBuilder<TIn, TOut> WithPreRule(Rules.IRule<TIn> rule)
    {
        AsyncRuleset.AddAsyncPreRule(rule.WrapAsync());
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPreRule(IRule<TIn> rule)
    {
        AsyncRuleset.AddAsyncPreRule(rule);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<Rules.IRule<TIn>> rules)
    {
        AsyncRuleset.AddAsyncPreRules(rules.Select(r => r.WrapAsync()));
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<IRule<TIn>> rules)
    {
        AsyncRuleset.AddAsyncPreRules(rules);
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> WithAsyncRule(string name)
      => new RuleBuilder<TIn, TOut>(this, name);

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncRule(IRule<TIn, TOut> rule)
    {
        AsyncRuleset.AddAsyncRule(rule);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithRule(Rules.IRule<TIn, TOut> rule)
    {
        AsyncRuleset.AddAsyncRule(rule.WrapAsync());
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithRules(IEnumerable<Rules.IRule<TIn, TOut>> rules)
    {
        AsyncRuleset.AddAsyncRules(rules.Select(r => r.WrapAsync()));
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<IRule<TIn, TOut>> rules)
    {
        AsyncRuleset.AddAsyncRules(rules);
        return this;
    }

    /// <inheritdoc />
    public IPostRuleBuilder<TIn, TOut> WithAsyncPostRule(string name)
      => new PostRuleBuilder<TIn, TOut>(this, name);

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPostRule(Rules.IRule<TOut> rule)
    {
        AsyncRuleset.AddAsyncPostRule(rule.WrapAsync());
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPostRule(IRule<TOut> rule)
    {
        AsyncRuleset.AddAsyncPostRule(rule);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<Rules.IRule<TOut>> rules)
    {
        AsyncRuleset.AddAsyncPostRules(rules.Select(r => r.WrapAsync()));
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<IRule<TOut>> rules)
    {
        AsyncRuleset.AddAsyncPostRules(rules);
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