using Rubric.Dependency;

namespace Rubric.Rules;

/// <summary>
///     An engine processing rule.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IRule<in TIn, in TOut> : IDependency
{
  /// <summary>
  ///     Determine whether this rule applies in the given context on the given input and output.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  /// <returns>Whether this rule should be applied.</returns>
  bool DoesApply(IEngineContext context, TIn input, TOut output);

  /// <summary>
  ///     Apply this rule in the given context on the given inputs and outputs.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  void Apply(IEngineContext context, TIn input, TOut output);
}

/// <summary>
///     An engine processing rule.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IRule<in T> : IDependency
{
  /// <summary>
  ///     Determine whether this rule applies in the given context on the given input and output.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <returns>Whether this rule should be applied.</returns>
  bool DoesApply(IEngineContext context, T input);

  /// <summary>
  ///     Apply this rule in the given context on the given inputs and outputs.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  void Apply(IEngineContext context, T input);
}
