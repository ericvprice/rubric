using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Rulesets.Probabilistic.Async;

public class Ruleset<TIn, TOut> : IRuleset<TIn, TOut>
{
  private readonly List<IRule<TOut>> _postprocessingRules;
  private readonly List<IRule<TIn>> _preprocessingRules;

  private readonly List<IRule<TIn, TOut>> _rules;

  public Ruleset()
  {
    _preprocessingRules = new();
    _rules = new();
    _postprocessingRules = new();
  }

  /// <inheritdoc />
  public IEnumerable<IRule<TIn>> PreRules => _preprocessingRules;

  /// <inheritdoc />
  public IEnumerable<IRule<TIn, TOut>> Rules => _rules;

  /// <inheritdoc />
  public IEnumerable<IRule<TOut>> PostRules => _postprocessingRules;

  /// <inheritdoc />
  public void AddPreRule(IRule<TIn> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _preprocessingRules.Add(rule);
  }

  /// <inheritdoc />
  public void AddPostRule(IRule<TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _postprocessingRules.Add(rule);
  }

  /// <inheritdoc />
  public void AddRule(IRule<TIn, TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  /// <inheritdoc />
  public void AddPreRules(IEnumerable<IRule<TIn>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _preprocessingRules.AddRange(rules);
  }

  /// <inheritdoc />
  public void AddPostRules(IEnumerable<IRule<TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _postprocessingRules.AddRange(rules);
  }

  /// <inheritdoc />
  public void AddRules(IEnumerable<IRule<TIn, TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}
