namespace Rubric.Builder.Async;

public interface IPreRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
    /// <summary>
    ///   Set the predicate function for this rule.
    /// </summary>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, Task<bool>> predicate);

    /// <summary>
    ///   Set the predicate function for this rule.
    /// </summary>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, CancellationToken, Task<bool>> predicate);

    /// <summary>
    ///   Set the action function for this rule.
    /// </summary>
    /// <param name="action">The action function.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, Task> action);

    /// <summary>
    ///   Set the action function for this rule.
    /// </summary>
    /// <param name="action">The action function.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, CancellationToken, Task> action);

    /// <summary>
    ///   Add a dependency by name.
    /// </summary>
    /// <param name="dep">The dependency name.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

    /// <summary>
    ///   Add a dependency by type.
    /// </summary>
    /// <param name="dep">The dependency type.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

    /// <summary>
    ///   Add a provided dependency by this rule.
    /// </summary>
    /// <param name="provides">The provided dependency name.</param>
    /// <returns>A fluent continuation.</returns>
    IPreRuleBuilder<TIn, TOut> ThatProvides(string provides);

    /// <summary>
    ///   End this rule builder.
    /// </summary>
    /// <returns>A fluent continuation for the parent engine.</returns>
    IEngineBuilder<TIn, TOut> EndRule();
}
