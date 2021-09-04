namespace Rubric.Rules;

public class Ruleset<TIn, TOut>
{
  private readonly List<IRule<TOut>> _postRules;
  private readonly List<IRule<TIn>> _preRules;
  private readonly List<IRule<TIn, TOut>> _rules;

  public Ruleset()
  {
    _preRules = new List<IRule<TIn>>();
    _rules = new List<IRule<TIn, TOut>>();
    _postRules = new List<IRule<TOut>>();
  }

  public IEnumerable<IRule<TIn>> PreRules => _preRules;

  public IEnumerable<IRule<TIn, TOut>> Rules => _rules;

  public IEnumerable<IRule<TOut>> PostRules => _postRules;

  public virtual void AddPreRule(IRule<TIn> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _preRules.Add(rule);
  }

  public virtual void AddPostRule(IRule<TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _postRules.Add(rule);
  }

  public virtual void AddRule(IRule<TIn, TOut> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  public virtual void AddPreRules(IEnumerable<IRule<TIn>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _preRules.AddRange(rules);
  }

  public virtual void AddPostRules(IEnumerable<IRule<TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _postRules.AddRange(rules);
  }

  public virtual void AddRules(IEnumerable<IRule<TIn, TOut>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}

public class Ruleset<T>
{
  private readonly List<IRule<T>> _rules;

  public Ruleset()
  {
    _rules = new List<IRule<T>>();
  }

  public IEnumerable<IRule<T>> Rules => _rules;

  public virtual void AddRule(IRule<T> rule)
  {
    if (rule == null) throw new ArgumentNullException(nameof(rule));
    _rules.Add(rule);
  }

  public virtual void AddRules(IEnumerable<IRule<T>> rules)
  {
    if (rules == null) throw new ArgumentNullException(nameof(rules));
    _rules.AddRange(rules);
  }
}