using System;
using Microsoft.Extensions.Logging;

namespace RulesEngine
{
    public static class EngineContextExtensions
    {
        public const string ENGINE_KEY = "_ENGINE";

        /// <summary>
        ///     Get the currently executing engine.
        /// </summary>
        /// <param name="context">The engine type</param>
        public static IRulesEngine GetEngine(this IEngineContext context)
            => context.Get<IRulesEngine>(ENGINE_KEY);

        /// <summary>
        ///     Get the strongly-typed currently executing engine.
        ///     This can be safely called after examining <see cref="IsAsync">IsAsync</see>.
        /// </summary>
        /// <param name="context">The engine context</param>
        public static IRulesEngine<TIn, TOut> GetEngine<TIn, TOut>(this IEngineContext context)
            where TIn : class
            where TOut : class
            => context.Get<IRulesEngine<TIn, TOut>>(ENGINE_KEY);

        /// <summary>
        ///     Get the strongly-typed currently executing asynchronous engine.
        ///     This can be safely called after examining <see cref="IsAsync">IsAsync</see>.
        /// </summary>
        /// <param name="context">The engine context.</param>
        public static IAsyncRulesEngine<TIn, TOut> GetAsyncEngine<TIn, TOut>(this IEngineContext context)
            where TIn : class
            where TOut : class
            => context.Get<IAsyncRulesEngine<TIn, TOut>>(ENGINE_KEY);

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
            => context.GetEngine().IsParallel;

        public static Type GetInputType(this IEngineContext context)
            => context.GetEngine().InputType;

        public static Type GetOutputType(this IEngineContext context)
            => context.GetEngine().OutputType;

    }
}