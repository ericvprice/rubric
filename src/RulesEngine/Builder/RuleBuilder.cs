using System;
using System.Collections.Generic;
using RulesEngine.Rules;
using static System.String;

namespace RulesEngine.Builder
{
    internal class RuleBuilder<TIn, TOut> : IRuleBuilder<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        private readonly List<string> _deps;
        private readonly string _name;

        private readonly EngineBuilder<TIn, TOut> _parentBuilder;
        private readonly List<string> _provides;
        private Action<IEngineContext, TIn, TOut> _action;
        private Func<IEngineContext, TIn, TOut, bool> _predicate;

        internal RuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name)
        {
            _parentBuilder = engineBuilder;
            _name = IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
            _provides = new List<string> { name };
            _deps = new List<string>();
        }


        public IEngineBuilder<TIn, TOut> EndRule()
        {
            _parentBuilder.Ruleset.AddRule(new LambdaRule<TIn, TOut>(_name, _predicate, _action, _deps, _provides));
            return _parentBuilder;
        }

        public IRuleBuilder<TIn, TOut> ThatProvides(string provides)
        {
            if (IsNullOrEmpty(provides)) throw new ArgumentException(nameof(provides));
            _provides.Add(provides);
            return this;
        }

        public IRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn, TOut> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public IRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
        {
            if (IsNullOrEmpty(dep)) throw new ArgumentException(nameof(dep));
            _deps.Add(dep);
            return this;
        }

        public IRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
        {
            _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
            return this;
        }

        public IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }
    }

    internal class RuleBuilder<T> : IRuleBuilder<T>
        where T : class
    {
        private readonly List<string> _deps;
        private readonly string _name;

        private readonly EngineBuilder<T> _parentBuilder;
        private readonly List<string> _provides;
        private Action<IEngineContext, T> _action;
        private Func<IEngineContext, T, bool> _predicate;

        internal RuleBuilder(EngineBuilder<T> engineBuilder, string name)
        {
            _parentBuilder = engineBuilder;
            _name = IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
            _provides = new List<string> { name };
            _deps = new List<string>();
        }


        public IEngineBuilder<T> EndRule()
        {
            _parentBuilder.Ruleset.AddRule(new LambdaRule<T>(_name, _predicate, _action, _deps, _provides));
            return _parentBuilder;
        }

        public IRuleBuilder<T> ThatProvides(string provides)
        {
            if (IsNullOrEmpty(provides)) throw new ArgumentException(nameof(provides));
            _provides.Add(provides);
            return this;
        }

        public IRuleBuilder<T> WithAction(Action<IEngineContext, T> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        public IRuleBuilder<T> ThatDependsOn(string dep)
        {
            if (IsNullOrEmpty(dep)) throw new ArgumentException(nameof(dep));
            _deps.Add(dep);
            return this;
        }

        public IRuleBuilder<T> ThatDependsOn(Type dep)
        {
            _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
            return this;
        }

        public IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }
    }
}