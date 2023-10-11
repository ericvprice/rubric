using Rubric.Dependency;

namespace Rubric.Rules.Probabilistic;

/// <summary>
///   An engine processing rule.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IRule<in TIn, in TOut> : IDependency
{
  /// <summary>
  ///   Return a value between 0 and 1 indicating the probability of this rule being applied
  ///   applies in the given context on the given input and output.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  /// <returns>Whether this rule should be applied.</returns>
  double DoesApply(IEngineContext context, TIn input, TOut output);

  /// <summary>
  ///   Apply this rule in the given context on the given input and output.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  void Apply(IEngineContext context, TIn input, TOut output);
}