using Microsoft.Extensions.Logging;
using Rubric.Async;

namespace Rubric;

public static class EngineContextExtensions
{
  public const string ENGINE_KEY = "__ENGINE";

  public const string TRACE_ID_KEY = "__TRACE_ID";

  public const string LAST_EXCEPTION_KEY = "__LAST_EXCEPTION";

  /// <summary>
  ///     Get the currently execution id.
  /// </summary>
  /// <param name="context">The target engine context.</param>
  /// <returns>A unique execution id.</returns>
  public static string GetTraceId(this IEngineContext context)
    => context.Get<string>(TRACE_ID_KEY);

  /// <summary>
  ///   Get the last engine exception thrown, if any.  Can be used to check the
  ///   exit status of the engine.
  /// </summary>
  /// <param name="context">The target engine context.</param>
  /// <returns>The last engine exception handled.</returns>
  public static EngineException GetLastException(this IEngineContext context)
    => context.ContainsKey(LAST_EXCEPTION_KEY)
        ? context.Get<EngineException>(LAST_EXCEPTION_KEY)
        : null;

  /// <summary>
  ///     Get the currently executing engine.
  /// </summary>
  /// <param name="context">The engine type</param>
  public static IRuleEngine GetEngine(this IEngineContext context)
      => context.Get<IRuleEngine>(ENGINE_KEY);

  /// <summary>
  ///     Get the strongly-typed currently executing engine.
  ///     This can be safely called after examining <see cref="IsAsync">IsAsync</see>.
  /// </summary>
  /// <param name="context">The engine context</param>
  public static IRuleEngine<TIn, TOut> GetEngine<TIn, TOut>(this IEngineContext context)
      where TIn : class
      where TOut : class
      => context.Get<IRuleEngine<TIn, TOut>>(ENGINE_KEY);

  /// <summary>
  ///     Get the strongly-typed currently executing asynchronous engine.
  ///     This can be safely called after examining <see cref="IsAsync">IsAsync</see>.
  /// </summary>
  /// <param name="context">The engine context.</param>
  public static IAsyncRuleEngine<TIn, TOut> GetAsyncEngine<TIn, TOut>(this IEngineContext context)
      where TIn : class
      where TOut : class
      => context.Get<IAsyncRuleEngine<TIn, TOut>>(ENGINE_KEY);

  /// <summary>
  ///     Get the currently executing engine's logger.
  /// </summary>
  /// <param name="context">the engine context.</param>
  public static ILogger GetLogger(this IEngineContext context)
      => context.GetEngine().Logger;

  /// <summary>
  ///     Get whether the currently executing engine is asynchronous.
  /// </summary>
  /// <param name="context">The engine context.</param>
  public static bool IsAsync(this IEngineContext context)
      => context.GetEngine().IsAsync;

  /// <summary>
  ///     Get whether the currently executing engine is executing rules in parallel.
  /// </summary>
  /// <param name="context">The engine context.</param>
  public static bool IsParallel(this IEngineContext context)
      => (context.GetEngine() as IAsyncRuleEngine)?.IsParallel ?? false;

  /// <summary>
  ///   Get the current engine's input type.
  /// </summary>
  /// <param name="context">The target context.</param>
  /// <returns>The current engine's input type.</returns>
  public static Type GetInputType(this IEngineContext context)
      => context.GetEngine().InputType;

  /// <summary>
  ///   Get the current engine's output type.
  /// </summary>
  /// <param name="context">The target context.</param>
  /// <returns>The current engine's output type.</returns>
  public static Type GetOutputType(this IEngineContext context)
      => context.GetEngine().OutputType;

}