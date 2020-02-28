using System.Threading.Tasks;
using RulesEngine.Dependency;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     A runtime-constructed asynchronous processing rule.
    /// </summary>
    /// <typeparam name="TIn">The engine input type.</typeparam>
    /// <typeparam name="TOut">The engine output type.</typeparam>
    public abstract class AsyncRule<TIn, TOut> : BaseDependency, IAsyncRule<TIn, TOut>
    {
        /// <inheritdoc />
        public override string Name => GetType().FullName;

        /// <inheritdoc />
        public abstract Task Apply(IEngineContext context, TIn input, TOut output);

        /// <inheritdoc />
        public abstract Task<bool> DoesApply(IEngineContext context, TIn input, TOut output);
    }

    /// <summary>
    ///     A runtime-constructed asynchronous processing rule.
    /// </summary>
    /// <typeparam name="TIn">The engine input type.</typeparam>
    /// <typeparam name="TOut">The engine output type.</typeparam>
    public abstract class AsyncRule<T> : BaseDependency, IAsyncRule<T>
        where T : class
    {
        /// <inheritdoc />
        public override string Name => GetType().FullName;

        /// <inheritdoc />
        public abstract Task Apply(IEngineContext context, T input);

        /// <inheritdoc />
        public abstract Task<bool> DoesApply(IEngineContext context, T input);
    }
}