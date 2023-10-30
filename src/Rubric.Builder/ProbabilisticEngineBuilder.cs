using Microsoft.Extensions.Logging;
using Rubric.Builder.Probabilistic;
using Rubric.Builder.Probabilistic.Implementation;

namespace Rubric;

/// <summary>
///   Starter static class for fluent engine building.
/// </summary>
public static class ProbabilisticEngineBuilder
{
  /// <summary>
  ///   Start building a synchronous engine for a single input type.
  /// </summary>
  /// <typeparam name="T">The input type.</typeparam>
  /// <param name="logger">An optional logger.</param>
  /// <returns>A fluent engine builder.</returns>
  public static IEngineBuilder<T> ForInput<T>(ILogger logger = null)
    where T : class
    => new EngineBuilder<T>(logger);

  /// <summary>
  ///   Start building a synchronous engine for input and output types.
  /// </summary>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <param name="logger">An optional logger.</param>
  /// <returns>A fluent engine builder.</returns>
  public static IEngineBuilder<TIn, TOut> ForInputAndOutput<TIn, TOut>(ILogger logger = null)
    where TIn : class
    where TOut : class
    => new EngineBuilder<TIn, TOut>(logger);

  /// <summary>
  ///   Start building an asynchronous engine for a single input type.
  /// </summary>
  /// <typeparam name="T">The input type.</typeparam>
  /// <param name="logger">An optional logger.</param>
  /// <returns>A fluent engine builder.</returns>
  public static Builder.Probabilistic.Async.IEngineBuilder<T> ForInputAsync<T>(ILogger logger = null)
    where T : class
    => new Builder.Probabilistic.Async.Implementation.EngineBuilder<T>(logger);

  /// <summary>
  ///   Start building an asynchronous engine for input and output types.
  /// </summary>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <param name="logger">An optional logger.</param>
  /// <returns>A fluent engine builder.</returns>
  public static Builder.Probabilistic.Async.IEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(
    ILogger logger = null)
    where TIn : class
    where TOut : class
    => new Builder.Probabilistic.Async.Implementation.EngineBuilder<TIn, TOut>(logger);
}