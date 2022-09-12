using Rubric.Async;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Builder.Async;

public interface IAsyncEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{

  /// <summary>
  ///   Start building a named preprocessing rule.
  /// </summary>
  /// <param name="name">The name of the preprocessing rule.</param>
  /// <returns>A preprocessing rule builder.</returns>
  IAsyncPreRuleBuilder<TIn, TOut> WithAsyncPreRule(string name);

  /// <summary>
  ///   Add a preprocessing rule to this engine.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

  /// <summary>
  ///   Add an async preprocessing rule to this engine.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncPreRule(IAsyncRule<TIn> rule);

  /// <summary>
  ///   Add multiple preprocessing rule to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rules);

  /// <summary>
  ///   Add multiple async preprocessing rule to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules);

  /// <summary>
  ///   Start building a named rule.
  /// </summary>
  /// <param name="name">The name of the rule.</param>
  /// <returns>A rule builder.</returns>
  IAsyncRuleBuilder<TIn, TOut> WithAsyncRule(string name);

  /// <summary>
  ///   Add an async rule to this engine.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncRule(IAsyncRule<TIn, TOut> rule);

  /// <summary>
  ///   Add a rule to this engine.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

  /// <summary>
  ///   Add multiple rules to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rules);

  /// <summary>
  ///   Add multiple async rules to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules);

  /// <summary>
  ///   Starting building an async postprocessing rule by name.
  /// </summary>
  /// <param name="name">The name of the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IAsyncPostRuleBuilder<TIn, TOut> WithAsyncPostRule(string name);

  /// <summary>
  ///   Add a postprocessing rule to this engine.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRule(IRule<TOut> rule);

  /// <summary>
  ///   Add an async postprocessing rule to this engine.
  /// </summary>
  /// <param name="rule">The rule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRule(IAsyncRule<TOut> rule);

  /// <summary>
  ///   Add multiple postprocessing rule to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rules);

  /// <summary>
  ///   Add multiple async postprocessing rule to this engine.
  /// </summary>
  /// <param name="rules">The rules to add.</param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules);

  /// <summary>
  ///   Set this engine to execute rules in parallel.
  /// </summary>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> AsParallel();

  /// <summary>
  ///   Set the exception handler for this engine.
  /// </summary>
  /// <param name="handler"></param>
  /// <returns>A fluent continuation.</returns>
  IAsyncEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler);

  /// <summary>
  ///    Finish building the engine and return the result.
  /// </summary>
  /// <returns>The completed engine.</returns>
  IAsyncRuleEngine<TIn, TOut> Build();
}
