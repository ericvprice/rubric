using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Rules;
using Rubric.Rulesets;

namespace Rubric.Builder;

internal class EngineBuilder<TIn, TOut> : IEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal Ruleset<TIn, TOut> Ruleset { get; } = new();

  internal ILogger Logger { get; }

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  public IPostRuleBuilder<TIn, TOut> WithPostRule(string name)
      => new PostRuleBuilder<TIn, TOut>(this, name);

  public IPreRuleBuilder<TIn, TOut> WithPreRule(string name)
      => new PreRuleBuilder<TIn, TOut>(this, name);

  public IRuleBuilder<TIn, TOut> WithRule(string name)
      => new RuleBuilder<TIn, TOut>(this, name);

  public IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule)
  {
    Ruleset.AddPostRule(rule);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule)
  {
    Ruleset.AddPreRule(rule);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
  {
    Ruleset.AddRule(rule);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler h)
  {
    ExceptionHandler = h;
    return this;
  }

  public IRuleEngine<TIn, TOut> Build() => new RuleEngine<TIn, TOut>(Ruleset, ExceptionHandler, Logger);

  public IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rules)
  {
    Ruleset.AddPreRules(rules);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rules)
  {
    Ruleset.AddRules(rules);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rules)
  {
    Ruleset.AddPostRules(rules);
    return this;
  }
}