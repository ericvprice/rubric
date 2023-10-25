using Rubric.Rules.Async;

namespace Rubric;

/// <summary>
///   Extension methods for wrapping and converting synchronous rules into asynchronous rules.
/// </summary>
public static class AsyncRuleExtensionMethods
{
  /// <summary>
  ///   Wrap an synchronous rule in an asynchronous wrapper.
  /// </summary>
  /// <param name="syncRule">The rule to wrap.</param>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <returns>An async wrapper for this rule.</returns>
  public static IRule<TIn, TOut> WrapAsync<TIn, TOut>(this Rules.IRule<TIn, TOut> syncRule) =>
    new AsyncRuleWrapper<TIn, TOut>(syncRule);

  /// <summary>
  ///   Wrap an synchronous rule in an asynchronous wrapper.
  /// </summary>
  /// <param name="syncRule">The rule to wrap.</param>
  /// <typeparam name="T">The input type.</typeparam>
  /// <returns>An async wrapper for this rule.</returns>
  public static IRule<T> WrapAsync<T>(this Rules.IRule<T> syncRule) =>
    new AsyncRuleWrapper<T>(syncRule);

  /// <summary>
  ///   Wrap an synchronous rule in an asynchronous wrapper.
  /// </summary>
  /// <param name="syncRule">The rule to wrap.</param>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <returns>An async wrapper for this rule.</returns>
  public static Rules.Probabilistic.Async.IRule<TIn, TOut> WrapAsync<TIn, TOut>(
    this Rules.Probabilistic.IRule<TIn, TOut> syncRule) =>
    new Rules.Probabilistic.Async.AsyncRuleWrapper<TIn, TOut>(syncRule);

  /// <summary>
  ///   Wrap an synchronous rule in an asynchronous wrapper.
  /// </summary>
  /// <param name="syncRule">The rule to wrap.</param>
  /// <typeparam name="T">The input type.</typeparam>
  /// <returns>An async wrapper for this rule.</returns>
  public static Rules.Probabilistic.Async.IRule<T> WrapAsync<T>(this Rules.Probabilistic.IRule<T> syncRule) =>
    new Rules.Probabilistic.Async.AsyncRuleWrapper<T>(syncRule);
}