using Rubric.Async;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Builder.Async;

public interface IAsyncEngineBuilder<T>
    where T : class
{

  /// <summary>
  ///   Start building a named rule.
  /// </summary>
  /// <param name="name">The name of the rule.</param>
  /// <returns>A rule builder.</returns>
  IAsyncRuleBuilder<T> WithRule(string name);

  /// <summary>
  ///   Add a rule to this engine.
  /// </summary>
  /// <param name="rule"></param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<T> WithRule(IRule<T> rule);

  /// <summary>
  ///   Add an asynchronous rule for this engine.
  /// </summary>
  /// <param name="rule">The rule.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<T> WithRule(IAsyncRule<T> rule);

  /// <summary>
  ///   Add multiple rules to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<T> WithRules(IEnumerable<IRule<T>> rules);

  /// <summary>
  ///   Add multiple asynchronous rules to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<T> WithRules(IEnumerable<IAsyncRule<T>> rules);

  /// <summary>
  ///   Set this engine to execute it's rules in parallel.
  /// </summary>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<T> AsParallel();

  /// <summary>
  ///   Set the exception handler for this engine.
  /// </summary>
  /// <param name="handler"></param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<T> WithExceptionHandler(IExceptionHandler handler);

  /// <summary>
  ///   Finish building the engine and return the result.
  /// </summary>
  /// <returns>The built engine.</returns>
  IAsyncRuleEngine<T> Build();
}
