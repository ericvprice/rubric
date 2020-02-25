using System;
using System.Collections.Generic;
using System.Linq;

namespace RulesEngine.Rules
{
    public class LambdaPrePostRule<T> : IPrePostRule<T>
    {
        private readonly Action<IEngineContext, T> _action;
        private readonly Func<IEngineContext, T, bool> _predicate;

        public LambdaPrePostRule(
            string name,
            Func<IEngineContext, T, bool> predicate,
            Action<IEngineContext, T> action,
            IEnumerable<string> dependencies = null,
            IEnumerable<string> provides = null
        )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _action = action ?? throw new ArgumentNullException(nameof(action));
            Provides = provides?.ToArray() ?? new string[0];
            Dependencies = dependencies?.ToArray() ?? new string[0];
        }


        public string Name { get; }

        public IEnumerable<string> Dependencies { get; }

        public IEnumerable<string> Provides { get; }

        public void Apply(IEngineContext context, T obj)
            => _action(context, obj);

        public bool DoesApply(IEngineContext context, T obj)
            => _predicate(context, obj);
    }
}