using System;
using System.Collections.Generic;

namespace RulesEngine.Rules
{
    /// <summary>
    ///     A runtime-constructed postprocessing rule.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    public class LambdaPostRule<TOut> : LambdaPrePostRule<TOut>, IPostRule<TOut>
    {
        public LambdaPostRule(
            string name,
            Func<IEngineContext, TOut, bool> predicate,
            Action<IEngineContext, TOut> action,
            IEnumerable<string> dependencies = null,
            IEnumerable<string> provides = null
        ) : base(name, predicate, action, dependencies, provides) { }
    }
}