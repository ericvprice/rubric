using Rubric.Rules;

namespace Rubric.Builder;

public interface IEngineBuilder<T>
    where T : class
{

  IRuleBuilder<T> WithRule(string name);

  IEngineBuilder<T> WithRule(IRule<T> rule);

  IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rule);

  IEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler);

  IRuleEngine<T> Build();
}
