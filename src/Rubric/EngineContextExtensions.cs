using Microsoft.Extensions.Logging;
using Rubric.Engines;

namespace Rubric;

/// <summary>
///   Provide access to well-known engine context entries.
/// </summary>
public static class EngineContextExtensions
{
  /// <summary>
  ///   Key for retrieving the current engine executing.
  /// </summary>
  private const string EngineKey = "__ENGINE";

  /// <summary>
  ///   Key for retrieving the current execution trace identifier.
  /// </summary>
  private const string TraceIdKey = "__TRACE_ID";

  /// <summary>
  ///   Key fo retrieving the last unhandled exception.
  /// </summary>
  private const string LastExceptionKey = "__LAST_EXCEPTION";

  /// <summary>
  ///   Key for retrieving the current execution-wide predicate cache.
  /// </summary>
  private const string ExecutionPredicateCacheKey = "__EX_PRED_CACHE";

  /// <summary>
  ///   Key for retrieving the current per-input predicate cache.
  /// </summary>
  private const string InputPredicateCacheKey = "__INPUT_PRED_CACHE";

  internal static void SetExecutionInfo(this IEngineContext context, IRuleEngine engine, string traceId)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    context[EngineKey] = engine;
    context[TraceIdKey] = traceId;
  }

  /// <summary>
  ///   Get the currently execution id.
  /// </summary>
  /// <param name="context">The target engine context.</param>
  /// <returns>A unique execution id.</returns>
  public static string GetTraceId(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetAs<string>(TraceIdKey);
  }

  /// <summary>
  ///   Get the last engine exception thrown, if any.  Can be used to check the
  ///   exit status of the engine.
  /// </summary>
  /// <param name="context">The target engine context.</param>
  /// <returns>The last engine exception handled.</returns>
  public static EngineException GetLastException(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.ContainsKey(LastExceptionKey)
      ? context.GetAs<EngineException>(LastExceptionKey)
      : null;
  }

  /// <summary>
  ///   Set the last engine exception thrown.
  /// </summary>
  /// <param name="context">The target engine context.</param>
  /// <param name="e">The exception to set.</param>
  public static void SetLastException(this IEngineContext context, Exception e)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    context[LastExceptionKey] = e;
  }

  /// <summary>
  ///   Clear the last exception thrown.
  /// </summary>
  /// <param name="context">The target engine context.</param>
  public static void ClearLastException(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    context.Remove(LastExceptionKey);
  }

  /// <summary>
  ///   Get the currently executing engine.
  /// </summary>
  /// <param name="context">The engine type</param>
  public static IRuleEngine GetEngine(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetAs<IRuleEngine>(EngineKey);
  }

  /// <summary>
  ///   Get the strongly-typed currently executing engine.
  ///   This can be safely called after examining <see cref="IsAsync">IsAsync</see>.
  /// </summary>
  /// <param name="context">The engine context</param>
  public static IRuleEngine<TIn, TOut> GetEngine<TIn, TOut>(this IEngineContext context)
    where TIn : class
    where TOut : class
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetAs<IRuleEngine<TIn, TOut>>(EngineKey);
  }

  /// <summary>
  ///   Get the strongly-typed currently executing asynchronous engine.
  ///   This can be safely called after examining <see cref="IsAsync">IsAsync</see>.
  /// </summary>
  /// <param name="context">The engine context.</param>
  public static Engines.Async.IRuleEngine<TIn, TOut> GetAsyncEngine<TIn, TOut>(this IEngineContext context)
    where TIn : class
    where TOut : class
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetAs<Engines.Async.IRuleEngine<TIn, TOut>>(EngineKey);
  }

  /// <summary>
  ///   Get the currently executing engine's logger.
  /// </summary>
  /// <param name="context">the engine context.</param>
  public static ILogger GetLogger(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetEngine().Logger;
  }

  /// <summary>
  ///   Get whether the currently executing engine is asynchronous.
  /// </summary>
  /// <param name="context">The engine context.</param>
  public static bool IsAsync(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetEngine().IsAsync;
  }

  /// <summary>
  ///   Get whether the currently executing engine is executing rules in parallel.
  /// </summary>
  /// <param name="context">The engine context.</param>
  public static bool IsParallel(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return (context.GetEngine() as Engines.Async.IRuleEngine)?.IsParallel ?? false;
  }

  /// <summary>
  ///   Get the current engine's input type.
  /// </summary>
  /// <param name="context">The target context.</param>
  /// <returns>The current engine's input type.</returns>
  public static Type GetInputType(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetEngine().InputType;
  }

  /// <summary>
  ///   Get the current engine's output type.
  /// </summary>
  /// <param name="context">The target context.</param>
  /// <returns>The current engine's output type.</returns>
  public static Type GetOutputType(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetEngine().OutputType;
  }

  /// <summary>
  ///   Get the current item predicate cache.
  /// </summary>
  /// <param name="context">The current engine execution context.</param>
  /// <returns>The cache.</returns>
  public static AsyncConcurrentDictionary<string, bool> GetInputPredicateCache(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetOrSet<AsyncConcurrentDictionary<string, bool>>(InputPredicateCacheKey, () => new());
  }

  /// <summary>
  ///   Get the current execution predicate cache.
  /// </summary>
  /// <param name="context">The current engine execution context.</param>
  /// <returns>The cache.</returns>
  public static AsyncConcurrentDictionary<string, bool> GetExecutionPredicateCache(this IEngineContext context)
  {
    if (context == null) throw new ArgumentNullException(nameof(context));
    return context.GetOrSet<AsyncConcurrentDictionary<string, bool>>(ExecutionPredicateCacheKey, () => new());
  }

  /// <summary>
  ///   Clear the per-item predicate execution cache.
  /// </summary>
  /// <param name="context"></param>
  internal static void ClearInputPredicateCache(this IEngineContext context) => context.GetInputPredicateCache().Clear();

  /// <summary>
  ///   Clear the per-execution predicate execution cache.
  /// </summary>
  /// <param name="context"></param>
  internal static void ClearExecutionPredicateCache(this IEngineContext context) => context.GetExecutionPredicateCache().Clear();

  internal static void ClearAllCaches(this IEngineContext context)
  {
    context.ClearExecutionPredicateCache();
    context.ClearInputPredicateCache();
  }

}