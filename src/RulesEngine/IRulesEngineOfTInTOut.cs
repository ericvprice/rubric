using RulesEngine.Rules;

namespace RulesEngine;

/// <summary>
///     A rule engine.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IRulesEngine<in TIn, in TOut> : IRulesEngine
    where TIn : class
    where TOut : class
{
  IEnumerable<IRule<TIn>> PreRules { get; }

  IEnumerable<IRule<TIn, TOut>> Rules { get; }

  IEnumerable<IRule<TOut>> PostRules { get; }

  /// <summary>
  ///     Apply the given input to the output object.
  /// </summary>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  /// <param name="context">An optional injected context.</param>
  void Apply(TIn input, TOut output, IEngineContext context = null);

  /// <summary>
  ///     Serially apply the given inputs to the output object.
  /// </summary>
  /// <param name="inputs">The input objects.</param>
  /// <param name="output">The output object.</param>
  /// <param name="context">An optional injected context.</param>
  void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null);

}