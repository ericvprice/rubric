using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Rulesets.Probabilistic.Async;

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
  void AddRule(IRule<T> rule);

  /// <summary>
  ///   Add rules to this ruleset.
  /// </summary>
  /// <param name="rules">The rules to adds.</param>
  void AddRules(IEnumerable<IRule<T>> rules);
}
