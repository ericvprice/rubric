using Rubric.Rules;

namespace Rubric.Builder;

public interface IEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  IPreRuleBuilder<TIn, TOut> WithPreRule(string name);

  IRuleBuilder<TIn, TOut> WithRule(string name);

  IPostRuleBuilder<TIn, TOut> WithPostRule(string name);

  IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

  IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

  IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

  IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rule);

  IEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rule);

  IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rule);

  IEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler);

  IRuleEngine<TIn, TOut> Build();
}
