using System;
using System.Collections.Generic;

namespace RulesEngine.Rules
{
    /// <summary>
    ///     A runtime constructed preprocessing rule.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    public class LambdaPreRule<TIn> : LambdaPrePostRule<TIn>, IPreRule<TIn>
    {
        public LambdaPreRule(
            string name,
            Func<IEngineContext, TIn, bool> predicate,
            Action<IEngineContext, TIn> action,
            IEnumerable<string> dependencies = null,
            IEnumerable<string> provides = null
        ) : base(name, predicate, action, dependencies, provides) { }
    }
}