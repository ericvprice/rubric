using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Builder;

public interface IAsyncEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  IAsyncEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

  IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

  IAsyncEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

  IAsyncPreRuleBuilder<TIn, TOut> WithPreRule(string name);

  IAsyncRuleBuilder<TIn, TOut> WithRule(string name);

  IAsyncPostRuleBuilder<TIn, TOut> WithPostRule(string name);

  IAsyncEngineBuilder<TIn, TOut> WithPreRule(IAsyncRule<TIn> rule);

  IAsyncEngineBuilder<TIn, TOut> WithRule(IAsyncRule<TIn, TOut> rule);

  IAsyncEngineBuilder<TIn, TOut> WithPostRule(IAsyncRule<TOut> rule);

  IAsyncEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules);

  IAsyncEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules);

  IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules);

  IAsyncEngineBuilder<TIn, TOut> AsParallel();

  IAsyncRuleEngine<TIn, TOut> Build();

  IAsyncEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler ignore);
}
