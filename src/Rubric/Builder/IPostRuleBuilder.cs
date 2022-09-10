namespace Rubric.Builder;

public interface IPostRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  /// <summary>
  ///   Set the predicate function for this rule.
  /// </summary>
  /// <param name="predicate">The predicate function.</param>
  /// <returns>A fluent continuation.</returns>
  IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, bool> predicate);

  /// <summary>
  ///   Set the action function for this rule.
  /// </summary>
  /// <param name="action">The action function.</param>
  /// <returns>A fluent continuation.</returns>
  IPostRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TOut> action);

  /// <summary>
  ///   Add a dependency by name.
  /// </summary>
  /// <param name="dep">The dependency name.</param>
  /// <returns>A fluent continuation.</returns>
  IPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

  /// <summary>
  ///   Add a dependency by type.
  /// </summary>
  /// <param name="type">The dependency type.</param>
  /// <returns>A fluent continuation.</returns>

  IPostRuleBuilder<TIn, TOut> ThatDependsOn(Type type);
  
  /// <summary>
  ///   Add a provided dependency by this rule.
  /// </summary>
  /// <param name="provides">The provided dependency name.</param>
  /// <returns>A fluent continuation.</returns>
  IPostRuleBuilder<TIn, TOut> ThatProvides(string provides);

  /// <summary>
  ///   End this rule builder.
  /// </summary>
  /// <returns>A fluent continuation for the parent engine.</returns>
  IEngineBuilder<TIn, TOut> EndRule();
}
