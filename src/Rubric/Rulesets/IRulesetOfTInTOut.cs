using Rubric.Rules;

namespace Rubric.Rulesets;

public interface IRuleset<TIn, TOut>
{

  IEnumerable<IRule<TIn>> PreRules { get; }
  IEnumerable<IRule<TIn, TOut>> Rules { get; }
  IEnumerable<IRule<TOut>> PostRules { get; }

  void AddPostRule(IRule<TOut> rule);
  void AddPostRules(IEnumerable<IRule<TOut>> rules);
  void AddPreRule(IRule<TIn> rule);
  void AddPreRules(IEnumerable<IRule<TIn>> rules);
  void AddRule(IRule<TIn, TOut> rule);
  void AddRules(IEnumerable<IRule<TIn, TOut>> rules);
}
