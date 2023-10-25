using Microsoft.Extensions.Logging;
using Rubric.Engines.Probabilistic.Async;
using Rubric.Rules.Probabilistic;

namespace Rubric.Builder.Probabilistic.Async;

/// <summary>
///   A fluent interface for building a rule engine.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public interface IEngineBuilder<T>
  where T : class
{
  /// <summary>
  ///   The logger to use for this engine.
  /// </summary>
  ILogger Logger { get; }

  /// <summary>
  ///   Whether this engine should process in parallel.
  /// </summary>
  bool IsParallel { get; }

  /// <summary>
  ///   The exception handler to use for this engine.
  /// </summary>
  IExceptionHandler ExceptionHandler { get; }

  /// <summary>
  ///   Start building a named rule.
  /// </summary>
  /// <param name="name">The name of the rule.</param>
  /// <returns>A rule builder.</returns>
  IRuleBuilder<T> WithRule(string name);

  /// <summary>
  ///   Add a rule to this engine.
  /// </summary>
  /// <param name="rule"></param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithRule(IRule<T> rule);

  /// <summary>
  ///   Add an asynchronous rule for this engine.
  /// </summary>
  /// <param name="rule">The rule.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithRule(Rules.Probabilistic.Async.IRule<T> rule);

  /// <summary>
  ///   Add multiple rules to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules);

  /// <summary>
  ///   Add multiple asynchronous rules to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithRules(IEnumerable<Rules.Probabilistic.Async.IRule<T>> rules);

  /// <summary>
  ///   Set this engine to execute it's rules in parallel.
  /// </summary>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> AsParallel();

  /// <summary>
  ///   Set the exception handler for this engine.
  /// </summary>
  /// <param name="handler"></param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler);

  /// <summary>
  ///   Finish building the engine and return the result.
  /// </summary>
  /// <returns>The built engine.</returns>
  IRuleEngine<T> Build();
}