using Rubric.Rules;

namespace Rubric.Builder;

public interface IEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  /// <summary>
  ///   Begin building a prerule by name.
  /// </summary>
  /// <param name="name">The name fo the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IPreRuleBuilder<TIn, TOut> WithPreRule(string name);

  /// <summary>
  ///   Add a prerule.
  /// </summary>
  /// <param name="rule">The prerule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

  /// <summary>
  ///   Add multiple prerules.
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
  ///   Begin building a postrule by name.
  /// </summary>
  /// <param name="name">The name fo the new rule.</param>
  /// <returns>A fluent rule builder.</returns>
  IPostRuleBuilder<TIn, TOut> WithPostRule(string name);

  /// <summary>
  ///   Add a postrule.
  /// </summary>
  /// <param name="rule">The postrule to add.</param>
  /// <returns>A fluent continuation.</returns>
  IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

  /// <summary>
  ///   Add multiple postrules.
  /// </summary>
  /// <param name="rules">The postrules to add.</param>
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
