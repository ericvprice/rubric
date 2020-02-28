using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;
using static System.String;

namespace RulesEngine.Builder
{
    internal class AsyncPreRuleBuilder<TIn, TOut> : IAsyncPreRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        private readonly List<string> _deps;
        private readonly string _name;
        private readonly AsyncEngineBuilder<TIn, TOut> _parentBuilder;
        private readonly List<string> _provides;
        private Func<IEngineContext, TIn, Task> _action;
        private Func<IEngineContext, TIn, Task<bool>> _predicate;

        internal AsyncPreRuleBuilder(AsyncEngineBuilder<TIn, TOut> engineBuilder, string name)
        {
            _parentBuilder = engineBuilder;
            _name = IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
            _provides = new List<string> { name };
            _deps = new List<string>();
        }


        public IAsyncEngineBuilder<TIn, TOut> EndRule()
        {
            _parentBuilder.AsyncRuleset.AddAsyncPreRule(
                new LambdaAsyncRule<TIn>(_name, _predicate, _action, _deps, _provides));
            return _parentBuilder;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> ThatProvides(string provides)
        {
            if (IsNullOrEmpty(provides)) throw new ArgumentException(provides);
            _provides.Add(provides);
            return this;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, Task> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
        {
            if (IsNullOrEmpty(dep)) throw new ArgumentException(nameof(dep));
            _deps.Add(dep);
            return this;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
        {
            _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
            return this;
        }

        public IAsyncPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, Task<bool>> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }
    }
}