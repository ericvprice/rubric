namespace Rubric.Builder.Async;

public interface IRuleBuilder<T>
  where T : class
{
    /// <summary>
    ///   Set the predicate function for this rule.
    /// </summary>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, Task<bool>> predicate);

    /// <summary>
    ///   Set the predicate function for this rule.
    /// </summary>
    /// <param name="predicate">The predicate function.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, CancellationToken, Task<bool>> predicate);

    /// <summary>
    ///   Set the action function for this rule.
    /// </summary>
    /// <param name="action">The action function.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> WithAction(Func<IEngineContext, T, Task> action);

    /// <summary>
    ///   Set the action function for this rule.
    /// </summary>
    /// <param name="action">The action function.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> WithAction(Func<IEngineContext, T, CancellationToken, Task> action);

    /// <summary>
    ///   Add a dependency by name.
    /// </summary>
    /// <param name="dep">The dependency name.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> ThatDependsOn(string dep);

    /// <summary>
    ///   Add a dependency by type.
    /// </summary>
    /// <param name="dep">The dependency type.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> ThatDependsOn(Type dep);

    /// <summary>
    ///   Add a provided dependency by this rule.
    /// </summary>
    /// <param name="provides">The provided dependency name.</param>
    /// <returns>A fluent continuation.</returns>
    IRuleBuilder<T> ThatProvides(string provides);

    /// <summary>
    ///   End this rule builder.
    /// </summary>
    /// <returns>A fluent continuation for the parent engine.</returns>
    IEngineBuilder<T> EndRule();
}