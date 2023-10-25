using Microsoft.Extensions.Logging;
using Rubric.Engines;
using Rubric.Rules;

namespace Rubric.Builder;

/// <summary>
///   A fluent interface for building a rule engine.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IEngineBuilder<T>
    where T : class
{

  /// <summary>
  ///   Begin building a rule by name.
  /// </summary>
  /// <param name="name">The name fo the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IRuleBuilder<T> WithRule(string name);

  /// <summary>
  ///   Add a rule.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithRule(IRule<T> rule);

  /// <summary>
  ///   Add multiple rules.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules);

  /// <summary>
  ///   Set the exception handler for this method.
  /// </summary>
  /// <param name="handler">The handler.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler);

  /// <summary>
  ///   Finish building the engine and return the result.
  /// </summary>
  /// <returns>The built engine.</returns>
  IRuleEngine<T> Build();

  /// <summary>
  ///   The logger to use for this engine.
  /// </summary>
  ILogger Logger { get; }

  /// <summary>
  ///   The exception handler to use for this engine.
  /// </summary>
  IExceptionHandler ExceptionHandler { get; }
}
