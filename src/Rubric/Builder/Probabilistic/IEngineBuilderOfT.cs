using Microsoft.Extensions.Logging;
using Rubric.Engines.Probabilistic;
using Rubric.Rules.Probabilistic;

namespace Rubric.Builder.Probabilistic;

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

    ILogger Logger { get; }

    IExceptionHandler ExceptionHandler { get; }
}
