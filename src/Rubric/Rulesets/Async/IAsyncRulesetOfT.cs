using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

public interface IAsyncRuleset<T>
{
  IEnumerable<IAsyncRule<T>> AsyncRules { get; }

  void AddAsyncRule(IAsyncRule<T> rule);

  void AddAsyncRules(IEnumerable<IAsyncRule<T>> rules);
}
