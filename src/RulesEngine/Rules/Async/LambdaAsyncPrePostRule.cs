using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RulesEngine.Rules.Async
{

    /// <summary>
    ///     A constructed rule utilizing lambdas for the predicate and action.
    /// </summary>
    /// <typeparam name="T">The input/output type.</typeparam>
    public class LambdaAsyncPrePostRule<T> : IAsyncPrePostRule<T>
    {
        private readonly Func<IEngineContext, T, Task<bool>> _predicate;

        private readonly Func<IEngineContext, T, Task> _body;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="name">The name for this rule.</param>
        /// <param name="predicate">The predicate for the DoesApply function call.</param>
        /// <param name="body">The action for the Apply function call.</param>
        /// <param name="dependencies">The dependencies for this rule.</param>
        /// <param name="provides">The dependencies this rules provides.</param>
        public LambdaAsyncPrePostRule(
            string name,
            Func<IEngineContext, T, Task<bool>> predicate,
            Func<IEngineContext, T, Task> body,
            IEnumerable<string> dependencies,
            IEnumerable<string> provides
        ) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _body = body ?? throw new ArgumentNullException(nameof(body));
            Dependencies = dependencies ?? new string[0];
            Provides = provides ?? new string[0];
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<string> Dependencies { get; }

        /// <inheritdoc />
        public IEnumerable<string> Provides { get; }

        /// <inheritdoc />
        public Task Apply(IEngineContext context, T obj) => _body(context, obj);

        /// <inheritdoc />
        public Task<bool> DoesApply(IEngineContext context, T obj) => _predicate(context, obj);
    }
}