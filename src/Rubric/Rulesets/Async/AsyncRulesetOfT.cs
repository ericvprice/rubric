using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

public class AsyncRuleset<T> : IAsyncRuleset<T>
{
  private readonly List<IAsyncRule<T>> _rules;

  public AsyncRuleset() => _rules = new();

  /// <inheritdoc />
  public IEnumerable<IAsyncRule<T>> AsyncRules => _rules;

  /// <inheritdoc />
  public void AddAsyncRule(IAsyncRule<T> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  /// <inheritdoc />
  public void AddAsyncRules(IEnumerable<IAsyncRule<T>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}
