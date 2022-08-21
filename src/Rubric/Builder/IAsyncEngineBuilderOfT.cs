using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Builder;

public interface IAsyncEngineBuilder<T>
    where T : class
{
  IAsyncEngineBuilder<T> WithRule(IRule<T> rule);

  IAsyncRuleBuilder<T> WithRule(string name);

  IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule);

  IAsyncEngineBuilder<T> WithRules(IEnumerable<IAsyncRule<T>> rule);

  IAsyncEngineBuilder<T> AsParallel();

  IAsyncEngineBuilder<T> WithExceptionHandler(IExceptionHandler ignore);

  IAsyncRuleEngine<T> Build();
}
