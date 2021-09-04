using Rubric.Rules;

namespace Rubric;

/// <summary>
///     A rule engine.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IRuleEngine<in T> : IRuleEngine
        where T : class
{
  IEnumerable<IRule<T>> Rules { get; }

  /// <summary>
  ///     Apply the given input to the output object.
  /// </summary>
  /// <param name="input">The input object.</param>
  /// <param name="context">An optional injected context.</param>
  void Apply(T input, IEngineContext context = null);

  /// <summary>
  ///     Serially apply the given inputs to the output object.
  /// </summary>
  /// <param name="inputs">The input objects.</param>
  /// <param name="context">An optional injected context.</param>
  void Apply(IEnumerable<T> inputs, IEngineContext context = null);

}