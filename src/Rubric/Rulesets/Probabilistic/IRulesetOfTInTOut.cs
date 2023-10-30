using Rubric.Rules.Probabilistic;

namespace Rubric.Rulesets.Probabilistic;

/// <summary>
///   A set of rules for a probabilistic rule engine.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IRuleset<TIn, TOut>
{
  /// <summary>
  ///   The preprocessing rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of the preprocessing rules.</value>
  IEnumerable<IRule<TIn>> PreRules { get; }

  /// <summary>
  ///   The rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of the preprocessing rules.</value>
  IEnumerable<IRule<TIn, TOut>> Rules { get; }

  /// <summary>
  ///   The postprocessing rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of the postprocessing rules.</value>
  IEnumerable<IRule<TOut>> PostRules { get; }

  /// <summary>
  ///   Add a new postprocessing rule.
  /// </summary>
  /// <param name="rule">The postprocessing rule to add.</param>
  void AddPostRule(IRule<TOut> rule);

  /// <summary>
  ///   Add postprocessing rules.
  /// </summary>
  /// <param name="rules">The postprocessing rule to add.</param>
  void AddPostRules(IEnumerable<IRule<TOut>> rules);

  /// <summary>
  ///   Add a new preprocessing rule.
  /// </summary>
  /// <param name="rule">The preprocessing rule to add.</param>
  void AddPreRule(IRule<TIn> rule);

  /// <summary>
  ///   Add new preprocessing rules.
  /// </summary>
  /// <param name="rules">The preprocessing rule to add.</param>
  void AddPreRules(IEnumerable<IRule<TIn>> rules);

  /// <summary>
  ///   Add a new rule.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  void AddRule(IRule<TIn, TOut> rule);

  /// <summary>
  ///   Add new rules.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  void AddRules(IEnumerable<IRule<TIn, TOut>> rules);
}