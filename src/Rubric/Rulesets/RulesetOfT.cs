using Rubric.Rules;

namespace Rubric.Rulesets;

public class Ruleset<T> : IRuleset<T> where T : class
{
  private readonly List<IRule<T>> _rules = new();

  /// <inheritdoc />
  public IEnumerable<IRule<T>> Rules => _rules;

  /// <inheritdoc />
  public virtual void AddRule(IRule<T> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  /// <inheritdoc />
  public virtual void AddRules(IEnumerable<IRule<T>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}