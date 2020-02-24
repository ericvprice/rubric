using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     A runtime-constructed asynchronous preprocessing rule.
    /// </summary>
    /// <typeparam name="TIn">The engine input type.</typeparam>
    public class LambdaAsyncPreRule<TIn> : LambdaAsyncPrePostRule<TIn>, IAsyncPreRule<TIn>
    {
        public LambdaAsyncPreRule(
            string name,
            Func<IEngineContext, TIn, Task<bool>> predicate,
            Func<IEngineContext, TIn, Task> body,
            IEnumerable<string> dependencies = null,
            IEnumerable<string> provides = null
        ) : base(name, predicate, body, dependencies, provides)
        {
        }
    }
}