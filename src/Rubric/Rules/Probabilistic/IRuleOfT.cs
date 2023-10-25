using Rubric.Dependency;

namespace Rubric.Rules.Probabilistic;

/// <summary>
///   An fluent interface for rule building.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IRule<in T> : IDependency
{
  /// <summary>
  ///   Return the predicate result caching behavior for this rule.
  /// </summary>
  PredicateCaching CacheBehavior { get; }

  /// <summary>
  ///   Return a value between 0 and 1 indicating the probability of this rule being applied
  ///   applies in the given context on the given input.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <returns>Whether this rule should be applied.</returns>
  double DoesApply(IEngineContext context, T input);

  /// <summary>
  ///   Apply this rule in the given context on the given inputs and outputs.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  void Apply(IEngineContext context, T input);
}