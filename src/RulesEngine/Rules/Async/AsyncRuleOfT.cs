using System.Threading;
using System.Threading.Tasks;
using RulesEngine.Dependency;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     An asynchronous processing rule.
    /// </summary>
    /// <typeparam name="TIn">The engine input type.</typeparam>
    /// <typeparam name="TOut">The engine output type.</typeparam>
    public abstract class AsyncRule<T> : BaseDependency, IAsyncRule<T>
        where T : class
    {
        /// <inheritdoc />
        public override string Name => GetType().FullName;

        /// <inheritdoc />
        public Task Apply(IEngineContext context, T input, CancellationToken token)
            => Apply(context, input);


        public abstract Task Apply(IEngineContext context, T input);

        /// <inheritdoc />
        public Task<bool> DoesApply(IEngineContext context, T input, CancellationToken token)
            => DoesApply(context, input);

        public abstract Task<bool> DoesApply(IEngineContext context, T input);
    }
}