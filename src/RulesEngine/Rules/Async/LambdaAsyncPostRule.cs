using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesEngine.Rules.Async
{
    /// <summary>
    ///     A runtime-constructed asynchronous postprocessing rule.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public class LambdaAsyncPostRule<TOut> : LambdaAsyncPrePostRule<TOut>, IAsyncPostRule<TOut>
    {
        public LambdaAsyncPostRule(
            string name,
            Func<IEngineContext, TOut, Task<bool>> predicate,
            Func<IEngineContext, TOut, Task> body,
            IEnumerable<string> dependencies = null,
            IEnumerable<string> provides = null
        ) : base(name, predicate, body, dependencies, provides) { }
    }
}