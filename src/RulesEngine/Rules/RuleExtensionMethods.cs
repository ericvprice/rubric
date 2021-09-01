using RulesEngine.Rules.Async;

namespace RulesEngine.Rules;

public static class RuleExtensionMethods
{
  /// <summary>
  ///     Wrap an synchronous rule in an asynchronous wrapper.
  /// </summary>
  /// <param name="syncRule">The rule to write.</param>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <returns>An async wrapper for this rule.</returns>
  public static IAsyncRule<TIn, TOut> WrapAsync<TIn, TOut>(this IRule<TIn, TOut> syncRule) =>
      new AsyncRuleWrapper<TIn, TOut>(syncRule);

  /// <summary>
  ///     Wrap an synchronous rule in an asynchronous wrapper.
  /// </summary>
  /// <param name="syncRule">The rule to write.</param>
  /// <typeparam name="T">The input type.</typeparam>
  /// <returns>An async wrapper for this rule.</returns>
  public static IAsyncRule<T> WrapAsync<T>(this IRule<T> syncRule) =>
      new AsyncRuleWrapper<T>(syncRule);
}