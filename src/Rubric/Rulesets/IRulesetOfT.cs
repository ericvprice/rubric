using Rubric.Rules;

namespace Rubric.Rulesets;

public interface IRuleset<T> where T : class
{
  IEnumerable<IRule<T>> Rules { get; }

  void AddRules(IEnumerable<IRule<T>> rules);
  void AddRule(IRule<T> rule);
}
