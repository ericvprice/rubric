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

  IAsyncEngineBuilder<TIn, TOut> AsParallel();

  IAsyncRuleEngine<TIn, TOut> Build();

  IAsyncEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler ignore);
}

public interface IAsyncEngineBuilder<T>
    where T : class
{
  IAsyncEngineBuilder<T> WithRule(IRule<T> rule);

  IAsyncRuleBuilder<T> WithRule(string name);

  IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule);

  IAsyncEngineBuilder<T> AsParallel();

  IAsyncEngineBuilder<T> WithExceptionHandler(IExceptionHandler ignore);

  IAsyncRuleEngine<T> Build();
}
