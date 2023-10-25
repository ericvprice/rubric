namespace Rubric.Builder.Probabilistic;

/// <summary>
///    An fluent interface for rule building.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  /// <summary>
  ///   Set the predicate function for this rule.
  /// </summary>
  /// <param name="predicate">The predicate function.</param>
  /// <returns>A fluent continuation.</returns>
  IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, double> predicate);

  /// <summary>
  ///   Set the action function for this rule.
  /// </summary>
  /// <param name="action">The action function.</param>
  /// <returns>A fluent continuation.</returns>
  IRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn, TOut> action);

  /// <summary>
  ///   Add a dependency by name.
  /// </summary>
  /// <param name="dep">The dependency name.</param>
  /// <returns>A fluent continuation.</returns>
  IRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

  /// <summary>
  ///   Add a dependency by type.
  /// </summary>
  /// <param name="dep">The dependency type.</param>
  /// <returns>A fluent continuation.</returns>
  IRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

  /// <summary>
  ///   Add a provided dependency by this rule.
  /// </summary>
  /// <param name="provides">The provided dependency name.</param>
  /// <returns>A fluent continuation.</returns>
  IRuleBuilder<TIn, TOut> ThatProvides(string provides);

  /// <summary>
  ///   Predicate result caching behavior.
  /// </summary>
  /// <param name="caching">The desired caching behavior</param>
  /// <returns>A fluent continuation.</returns>
  IRuleBuilder<TIn, TOut> WithCaching(PredicateCaching caching);

  /// <summary>
  ///   End this rule builder.
  /// </summary>
  /// <returns>A fluent continuation for the parent engine.</returns>
  IEngineBuilder<TIn, TOut> EndRule();
}