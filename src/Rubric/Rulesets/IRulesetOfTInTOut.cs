using Rubric.Rules;

namespace Rubric.Rulesets;

public interface IRuleset<TIn, TOut>
{
  IEnumerable<IRule<TIn>> PreRules { get; }
  IEnumerable<IRule<TIn, TOut>> Rules { get; }
  IEnumerable<IRule<TOut>> PostRules { get; }
}
