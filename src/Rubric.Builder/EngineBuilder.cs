using Microsoft.Extensions.Logging;
using Rubric.Builder.Implementation;

namespace Rubric.Builder;

/// <summary>
///   Starter static class for fluent engine building.
/// </summary>
public static class EngineBuilder
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
  public static Builder.Async.IEngineBuilder<T> ForInputAsync<T>(ILogger logger = null)
    where T : class
    => new Builder.Async.Implementation.EngineBuilder<T>(logger);

  /// <summary>
  ///   Start building an asynchronous engine for input and output types.
  /// </summary>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <param name="logger">An optional logger.</param>
  /// <returns>A fluent engine builder.</returns>
  public static Builder.Async.IEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(ILogger logger = null)
    where TIn : class
    where TOut : class
    => new Builder.Async.Implementation.EngineBuilder<TIn, TOut>(logger);

  /// <summary>
  ///   Return a builder using an existing rule engine as a base.
  ///   The builder returned will have its logger, exception handler, and rules copied from the provided engine.
  /// </summary>
  /// <typeparam name="T">The engine input type.</typeparam>
  /// <param name="baseEngine">The engine used to initialize the builder.</param>
  /// <returns>A builder based on the existing engine.</returns>
  public static IEngineBuilder<T> FromEngine<T>(Engines.IRuleEngine<T> baseEngine) where T : class
    => new EngineBuilder<T>(baseEngine.Logger)
       .WithRules(baseEngine.Rules)
       .WithExceptionHandler(baseEngine.ExceptionHandler);

  /// <summary>
  ///   Return a builder using an existing rule engine as a base.
  ///   The builder returned will have its logger, exception handler, and rules copied from the provided engine.
  /// </summary>
  /// <typeparam name="TIn">The engine input type.</typeparam>
  /// <typeparam name="TOut">The engine output type.</typeparam>
  /// <param name="baseEngine">The engine used to initialize the builder.</param>
  /// <returns>A builder based on the existing engine.</returns>
  public static IEngineBuilder<TIn, TOut> FromEngine<TIn, TOut>(Engines.IRuleEngine<TIn, TOut> baseEngine) where TIn : class where TOut : class
    => new EngineBuilder<TIn, TOut>(baseEngine.Logger)
       .WithPreRules(baseEngine.PreRules)
       .WithRules(baseEngine.Rules)
       .WithPostRules(baseEngine.PostRules)
       .WithExceptionHandler(baseEngine.ExceptionHandler);

  /// <summary>
  ///   Return a builder using an existing rule engine as a base.
  ///   The builder returned will have its logger, exception handler, rules, and its parallelization copied from the provided engine.
  /// </summary>
  /// <typeparam name="T">The engine input type.</typeparam>
  /// <param name="baseEngine">The engine used to initialize the builder.</param>
  /// <returns>A builder based on the existing engine.</returns>
  public static Builder.Async.IEngineBuilder<T> FromEngine<T>(Engines.Async.IRuleEngine<T> baseEngine) where T : class
    => new Builder.Async.Implementation.EngineBuilder<T>(baseEngine.Logger, baseEngine.IsParallel)
       .WithRules(baseEngine.Rules)
       .WithExceptionHandler(baseEngine.ExceptionHandler);

  /// <summary>
  ///   Return a builder using an existing rule engine as a base.
  ///   The builder returned will have its logger, exception handler, rules, and its parallelization copied from the provided engine.
  /// </summary>
  /// <typeparam name="TIn">The engine input type.</typeparam>
  /// <typeparam name="TOut">The engine output type.</typeparam>
  /// <param name="baseEngine">The engine used to initialize the builder.</param>
  /// <returns>A builder based on the existing engine.</returns>
  public static Builder.Async.IEngineBuilder<TIn, TOut> FromEngine<TIn, TOut>(Engines.Async.IRuleEngine<TIn, TOut> baseEngine)
    where TIn : class where TOut : class
    => new Builder.Async.Implementation.EngineBuilder<TIn, TOut>(baseEngine.Logger, baseEngine.IsParallel)
       .WithPreRules(baseEngine.PreRules)
       .WithRules(baseEngine.Rules)
       .WithPostRules(baseEngine.PostRules)
       .WithExceptionHandler(baseEngine.ExceptionHandler);
}