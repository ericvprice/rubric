using Rubric.Dependency;

namespace Rubric.Rules.Async;

/// <summary>
///   An asynchronous engine processing rule.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public interface IRule<in TIn, in TOut> : IDependency
{
  /// <summary>
  ///   Return the predicate result caching behavior for this rule.
  /// </summary>
  PredicateCaching CacheBehavior { get; }

  /// <summary>
  ///   Whether this rule should apply to the given input, output, and
  ///   execution context.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  /// <param name="token">An optional cancellation token.</param>
  /// <returns>An awaitable task returning whether this rule should apply.</returns>
  Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken token);

  /// <summary>
  ///   Apply this rule on the given input and output objects.
  /// </summary>
  /// <param name="context">The execution context.</param>
  /// <param name="input">The input object.</param>
  /// <param name="output">The output object.</param>
  /// <param name="token">An optional cancellation token.</param>
  /// <returns>An awaitable task.</returns>
  Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken token);
}