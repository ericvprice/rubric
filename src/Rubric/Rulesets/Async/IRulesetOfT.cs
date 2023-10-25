using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

/// <summary>
///   A set of rules for a rule engine.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IRuleset<T>
{

  /// <summary>
  ///   The rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of rule.</value>
  IEnumerable<IRule<T>> Rules { get; }

  /// <summary>
  ///   Add a rule to this ruleset.
  /// </summary>
  /// <param name="rule">The ruleset to add.</param>
  void AddAsyncRule(IRule<T> rule);

  /// <summary>
  ///   Add rules to this ruleset.
  /// </summary>
  /// <param name="rules">The rules to adds.</param>
  void AddAsyncRules(IEnumerable<IRule<T>> rules);
}
