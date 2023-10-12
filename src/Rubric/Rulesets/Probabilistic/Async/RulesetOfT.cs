using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Rulesets.Probabilistic.Async;

public class Ruleset<T> : IRuleset<T>
{
  private readonly List<IRule<T>> _rules = new();

  /// <inheritdoc />
  public IEnumerable<IRule<T>> Rules => _rules;

  /// <inheritdoc />
  public void AddRule(IRule<T> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  /// <inheritdoc />
  public void AddRules(IEnumerable<IRule<T>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}
