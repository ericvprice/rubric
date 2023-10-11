using Rubric.Dependency;

namespace Rubric.Rules;

/// <summary>
///   An engine processing rule.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IRule<in T> : IDependency
{
  /// <summary>
  ///   Determine whether this rule applies in the given context on the given input.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <returns>Whether this rule should be applied.</returns>
  bool DoesApply(IEngineContext context, T input);

  /// <summary>
  ///   Apply this rule in the given context on the given input.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  void Apply(IEngineContext context, T input);
}
