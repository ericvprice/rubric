using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

public interface IAsyncRuleset<TIn, TOut>
{
  /// <summary>
  ///   The preprocessing rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of the preprocessing rules.</value>
  IEnumerable<IAsyncRule<TIn>> AsyncPreRules { get; }

  /// <summary>
  ///   The rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of the processing rules.</value>
  IEnumerable<IAsyncRule<TIn, TOut>> AsyncRules { get; }

  /// <summary>
  ///   The postprocessing rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of the postprocessing rules.</value>
  IEnumerable<IAsyncRule<TOut>> AsyncPostRules { get; }

  /// <summary>
  ///   Add a new postprocessing rule.
  /// </summary>
  /// <param name="rule">The postprocessing rule to add.</param>
  void AddAsyncPostRule(IAsyncRule<TOut> rule);

  /// <summary>
  ///   Add new postprocessing rules.
  /// </summary>
  /// <param name="rule">The postprocessing rules to add.</param>
  void AddAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules);

  /// <summary>
  ///   Add a new preprocessing rule.
  /// </summary>
  /// <param name="rule">The preprocessing rule to add.</param>
  void AddAsyncPreRule(IAsyncRule<TIn> rule);

  /// <summary>
  ///   Add new preprocessing rules.
  /// </summary>
  /// <param name="rule">The preprocessing rules to add.</param>
  void AddAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules);

  /// <summary>
  ///   Add a new rule.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  void AddAsyncRule(IAsyncRule<TIn, TOut> rule);

  /// <summary>
  ///   Add new rules.
  /// </summary>
  /// <param name="rule">The rules to add.</param>
  void AddAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules);

}
