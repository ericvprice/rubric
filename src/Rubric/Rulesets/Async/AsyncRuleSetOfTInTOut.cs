using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

public class AsyncRuleset<TIn, TOut> : IAsyncRuleset<TIn, TOut>
{
  private readonly List<IAsyncRule<TOut>> _postprocessingRules;
  private readonly List<IAsyncRule<TIn>> _preprocessingRules;

  private readonly List<IAsyncRule<TIn, TOut>> _rules;

  public AsyncRuleset()
  {
    _preprocessingRules = new();
    _rules = new();
    _postprocessingRules = new();
  }

  /// <inheritdoc />
  public IEnumerable<IAsyncRule<TIn>> AsyncPreRules => _preprocessingRules;

  /// <inheritdoc />
  public IEnumerable<IAsyncRule<TIn, TOut>> AsyncRules => _rules;

  /// <inheritdoc />
  public IEnumerable<IAsyncRule<TOut>> AsyncPostRules => _postprocessingRules;

  /// <inheritdoc />
  public void AddAsyncPreRule(IAsyncRule<TIn> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _preprocessingRules.Add(rule);
  }

  /// <inheritdoc />
  public void AddAsyncPostRule(IAsyncRule<TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _postprocessingRules.Add(rule);
  }

  /// <inheritdoc />
  public void AddAsyncRule(IAsyncRule<TIn, TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  /// <inheritdoc />
  public void AddAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _preprocessingRules.AddRange(rules);
  }

  /// <inheritdoc />
  public void AddAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _postprocessingRules.AddRange(rules);
  }

  /// <inheritdoc />
  public void AddAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}
