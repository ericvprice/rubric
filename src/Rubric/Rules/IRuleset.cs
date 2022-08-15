namespace Rubric.Rules;

public interface IRuleset<TIn>
{
  IEnumerable<IRule<TIn>> Rules { get; }
}

public interface IRuleset<TIn, TOut>
{
  IEnumerable<IRule<TIn>> PreRules { get; }
  IEnumerable<IRule<TIn, TOut>> Rules { get; }
  IEnumerable<IRule<TOut>> PostRules { get; }
}
