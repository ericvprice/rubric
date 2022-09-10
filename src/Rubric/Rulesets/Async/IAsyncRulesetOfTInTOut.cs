using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

public interface IAsyncRuleset<TIn, TOut>
{
  IEnumerable<IAsyncRule<TIn>> AsyncPreRules { get; }
  IEnumerable<IAsyncRule<TIn, TOut>> AsyncRules { get; }
  IEnumerable<IAsyncRule<TOut>> AsyncPostRules { get; }

  void AddAsyncPostRule(IAsyncRule<TOut> rule);
  void AddAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules);
  void AddAsyncPreRule(IAsyncRule<TIn> rule);
  void AddAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules);
  void AddAsyncRule(IAsyncRule<TIn, TOut> rule);
  void AddAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules);
}
