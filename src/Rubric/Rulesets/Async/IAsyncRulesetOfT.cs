using Rubric.Rules.Async;

namespace Rubric.Rulesets.Async;

public interface IAsyncRuleset<T>
{
  /// <summary>
  ///   The rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of rule.</value>
  IEnumerable<IAsyncRule<T>> AsyncRules { get; }

  /// <summary>
  ///   Add a rule to this ruleset.
  /// </summary>
  /// <param name="rule">The ruleset to add.</param>
  void AddAsyncRule(IAsyncRule<T> rule);

  /// <summary>
  ///   Add rules to this ruleset.
  /// </summary>
  /// <param name="rules">The rules to adds.</param>
  void AddAsyncRules(IEnumerable<IAsyncRule<T>> rules);
}
