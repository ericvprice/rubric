using Rubric.Engines.Async;
using Rubric.Rules.Async;

namespace Rubric.Builder.Async;

public interface IEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{

    /// <summary>
    ///   Start building a named preprocessing rule.
    /// </summary>
    /// <param name="name">The name of the preprocessing rule.</param>
    /// <returns>A preprocessing rule builder.</returns>
    IPreRuleBuilder<TIn, TOut> WithPreRule(string name);

    /// <summary>
    ///   Add a preprocessing rule to this engine.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPreRule(Rules.IRule<TIn> rule);

    /// <summary>
    ///   Add an async preprocessing rule to this engine.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule);

    /// <summary>
    ///   Add multiple preprocessing rule to this engine.
    /// </summary>
    /// <param name="rules">The rules to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<Rules.IRule<TIn>> rules);

    /// <summary>
    ///   Add multiple async preprocessing rule to this engine.
    /// </summary>
    /// <param name="rules">The rules to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPreRules(IEnumerable<IRule<TIn>> rules);

    /// <summary>
    ///   Start building a named rule.
    /// </summary>
    /// <param name="name">The name of the rule.</param>
    /// <returns>A rule builder.</returns>
    IRuleBuilder<TIn, TOut> WithRule(string name);

    /// <summary>
    ///   Add an async rule to this engine.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule);

    /// <summary>
    ///   Add a rule to this engine.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithRule(Rules.IRule<TIn, TOut> rule);

    /// <summary>
    ///   Add multiple rules to this engine.
    /// </summary>
    /// <param name="rules">The rules to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithRules(IEnumerable<Rules.IRule<TIn, TOut>> rules);

    /// <summary>
    ///   Add multiple async rules to this engine.
    /// </summary>
    /// <param name="rules">The rules to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithRules(IEnumerable<IRule<TIn, TOut>> rules);

    /// <summary>
    ///   Starting building an async postprocessing rule by name.
    /// </summary>
    /// <param name="name">The name of the new rule.</param>
    /// <returns>A fluent rule builder.</returns>
    IPostRuleBuilder<TIn, TOut> WithPostRule(string name);

    /// <summary>
    ///   Add a postprocessing rule to this engine.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPostRule(Rules.IRule<TOut> rule);

    /// <summary>
    ///   Add an async postprocessing rule to this engine.
    /// </summary>
    /// <param name="rule">The rule to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule);

    /// <summary>
    ///   Add multiple postprocessing rule to this engine.
    /// </summary>
    /// <param name="rules">The rules to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<Rules.IRule<TOut>> rules);

    /// <summary>
    ///   Add multiple async postprocessing rule to this engine.
    /// </summary>
    /// <param name="rules">The rules to add.</param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithPostRules(IEnumerable<IRule<TOut>> rules);

    /// <summary>
    ///   Set this engine to execute rules in parallel.
    /// </summary>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> AsParallel();

    /// <summary>
    ///   Set the exception handler for this engine.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns>A fluent continuation.</returns>
    IEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler handler);

    /// <summary>
    ///    Finish building the engine and return the result.
    /// </summary>
    /// <returns>The completed engine.</returns>
    IRuleEngine<TIn, TOut> Build();
}
