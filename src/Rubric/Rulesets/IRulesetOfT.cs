using Rubric.Rules;

namespace Rubric.Rulesets;

/// <summary>
///   A set of rules for a rule engine.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IRuleset<T> where T : class
{
  /// <summary>
  ///   The rules in this ruleset.
  /// </summary>
  /// <value>An enumeration of rules.</value>
  IEnumerable<IRule<T>> Rules { get; }

  /// <summary>
  ///   Add a rule to this ruleset.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  void AddRule(IRule<T> rule);

  /// <summary>
  ///   Add rules to this ruleset.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  void AddRules(IEnumerable<IRule<T>> rules);
}