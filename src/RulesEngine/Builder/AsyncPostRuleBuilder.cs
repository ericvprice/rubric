using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;
using static System.String;

namespace RulesEngine.Builder
{
    internal class AsyncPostRuleBuilder<TIn, TOut> : IAsyncPostRuleBuilder<TIn, TOut>
    {
        private readonly AsyncEngineBuilder<TIn, TOut> _parentBuilder;
        private readonly string _name;
        private readonly List<string> _provides;
        private readonly List<string> _deps;
        private Func<IEngineContext, TOut, Task> _action;
        private Func<IEngineContext, TOut, Task<bool>> _predicate;

        internal AsyncPostRuleBuilder(AsyncEngineBuilder<TIn, TOut> engineBuilder, string name)
        {
            _parentBuilder = engineBuilder;
            _name = IsNullOrWhiteSpace(name) ? throw new ArgumentException(nameof(name)) : name;
            _provides = new List<string> { name };
            _deps = new List<string>();
        }


        public IAsyncEngineBuilder<TIn, TOut> EndRule()
        {
            _parentBuilder.AsyncRuleset.AddAsyncPostRule(new LambdaAsyncPostRule<TOut>(_name, _predicate, _action, _deps, _provides));
            return _parentBuilder;
        }

        public IAsyncPostRuleBuilder<TIn, TOut> ThatProvides(string provides)
        {
            if (IsNullOrWhiteSpace(provides)) throw new ArgumentException(nameof(provides));
            _provides.Add(provides);
            return this;
        }

        public IAsyncPostRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TOut, Task> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(Action));
            return this;
        }

        public IAsyncPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
        {
            if (IsNullOrWhiteSpace(dep)) throw new ArgumentException(nameof(dep));
            _deps.Add(dep);
            return this;
        }

        public IAsyncPostRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
        {
            _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
            return this;
        }

        public IAsyncPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, Task<bool>> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }
    }
}