using System.Threading.Tasks;
using RulesEngine.Dependency;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     Abstract rule suitable for extension using attributes
    ///     for declarative dependencies.
    /// </summary>
    /// <typeparam name="T">The input/output type.</typeparam>
    public abstract class AsyncPrePostRule<T> : BaseDependency, IAsyncPrePostRule<T>
    {
        /// <inheritdoc />
        public override string Name => GetType().FullName;

        /// <inheritdoc />
        public abstract Task Apply(IEngineContext context, T obj);

        /// <inheritdoc />
        public abstract Task<bool> DoesApply(IEngineContext context, T obj);
    }
}