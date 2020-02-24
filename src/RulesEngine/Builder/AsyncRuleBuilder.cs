using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;
using static System.String;

namespace RulesEngine.Builder
{
    internal class AsyncRuleBuilder<TIn, TOut> : IAsyncRuleBuilder<TIn, TOut>
    {

        private readonly AsyncEngineBuilder<TIn, TOut> _parentBuilder;
        private readonly string _name;
        private readonly List<string> _provides;
        private readonly List<string> _deps;
        private Func<IEngineContext, TIn, TOut, Task> _action;
        private Func<IEngineContext, TIn, TOut, Task<bool>> _predicate;

        internal AsyncRuleBuilder(AsyncEngineBuilder<TIn, TOut> engineBuilder, string name)
        {
            _parentBuilder = engineBuilder;
            _name = IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
            _provides = new List<string> { name };
            _deps = new List<string>();
        }


        public IAsyncEngineBuilder<TIn, TOut> EndRule()
        {
            _parentBuilder.AsyncRuleset.AddAsyncRule(new LambdaAsyncRule<TIn, TOut>(_name, _predicate, _action, _deps, _provides));
            return _parentBuilder;
        }

        public IAsyncRuleBuilder<TIn, TOut> ThatProvides(string provides)
        {
            if (IsNullOrEmpty(provides)) throw new ArgumentException(nameof(provides));
            _provides.Add(provides);
            return this;
        }

        public IAsyncRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, Task> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
        {
            if (IsNullOrEmpty(dep)) throw new ArgumentException(nameof(dep));
            _deps.Add(dep);
            return this;
        }

        public IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
        {
            _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
            return this;
        }

        public IAsyncRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, Task<bool>> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }
    }
}