using Microsoft.Extensions.Logging;
using Rubric.Engines.Probabilistic;
using Rubric.Rules.Probabilistic;

namespace Rubric.Builder.Probabilistic;

/// <summary>
///   A fluent interface for building a probabilistic rule engine.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IEngineBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  /// <summary>
  ///   The logger to use for this engine.
  /// </summary>
  ILogger Logger { get; }

  /// <summary>
  ///   The exception handler to use for this engine.
  /// </summary>
  IExceptionHandler ExceptionHandler { get; }

  /// <summary>
  ///   Begin building a preprocessing rule by name.
  /// </summary>
  /// <param name="name">The name fo the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IPreRuleBuilder<TIn, TOut> WithPreRule(string name);

  /// <summary>
  ///   Add a preprocessing rule.
  /// </summary>
  /// <param name="rule">The preprocessing rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

  /// <summary>
  ///   Add multiple preprocessing rules.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rules);

  /// <summary>
  ///   Begin building a rule by name.
  /// </summary>
  /// <param name="name">The name fo the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IRuleBuilder<TIn, TOut> WithRule(string name);

  /// <summary>
  ///   Add a rule.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

  /// <summary>
  ///   Add multiple rules.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rules);

  /// <summary>
  ///   Begin building a postprocessing rule by name.
  /// </summary>
  /// <param name="name">The name fo the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IPostRuleBuilder<TIn, TOut> WithPostRule(string name);

  /// <summary>
  ///   Add a postprocessing rule.
  /// </summary>
  /// <param name="rule">The postprocessing rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

  /// <summary>
  ///   Add multiple postprocessing rules.
  /// </summary>
  /// <param name="rules">The postprocessing rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rules);

  /// <summary>
  ///   Set the exception handler for this method.
  /// </summary>
  /// <param name="handler">The handler.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler);

  /// <summary>
  ///   Finish building the engine and return the result.
  /// </summary>
  /// <returns>The built engine.</returns>
  IRuleEngine<TIn, TOut> Build();
}