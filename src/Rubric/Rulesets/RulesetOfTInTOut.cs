using Rubric.Rules;

namespace Rubric.Rulesets;

public class Ruleset<TIn, TOut> : IRuleset<TIn, TOut>
{
  private readonly List<IRule<TOut>> _postRules = new();
  private readonly List<IRule<TIn>> _preRules = new();
  private readonly List<IRule<TIn, TOut>> _rules = new();

  /// <inheritdoc />
  public IEnumerable<IRule<TIn>> PreRules => _preRules;

  /// <inheritdoc />
  public IEnumerable<IRule<TIn, TOut>> Rules => _rules;

  /// <inheritdoc />
  public IEnumerable<IRule<TOut>> PostRules => _postRules;


  public void AddPreRule(IRule<TIn> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _preRules.Add(rule);
  }

  public void AddPostRule(IRule<TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _postRules.Add(rule);
  }

  public void AddRule(IRule<TIn, TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  public void AddPreRules(IEnumerable<IRule<TIn>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _preRules.AddRange(rules);
  }

  public void AddPostRules(IEnumerable<IRule<TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _postRules.AddRange(rules);
  }

  public void AddRules(IEnumerable<IRule<TIn, TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}
