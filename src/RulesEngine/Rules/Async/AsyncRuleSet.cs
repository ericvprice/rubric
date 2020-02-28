using System;
using System.Collections.Generic;

namespace RulesEngine.Rules.Async
{
    public class AsyncRuleset<TIn, TOut>
    {
        private readonly List<IAsyncRule<TOut>> _postprocessingRules;
        private readonly List<IAsyncRule<TIn>> _preprocessingRules;

        private readonly List<IAsyncRule<TIn, TOut>> _rules;

        public AsyncRuleset()
        {
            _preprocessingRules = new List<IAsyncRule<TIn>>();
            _rules = new List<IAsyncRule<TIn, TOut>>();
            _postprocessingRules = new List<IAsyncRule<TOut>>();
        }

        public IEnumerable<IAsyncRule<TIn>> AsyncPreRules => _preprocessingRules;

        public IEnumerable<IAsyncRule<TIn, TOut>> AsyncRules => _rules;

        public IEnumerable<IAsyncRule<TOut>> AsyncPostRules => _postprocessingRules;

        public void AddAsyncPreRule(IAsyncRule<TIn> rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _preprocessingRules.Add(rule);
        }

        public void AddAsyncPostRule(IAsyncRule<TOut> rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _postprocessingRules.Add(rule);
        }

        public void AddAsyncRule(IAsyncRule<TIn, TOut> rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public void AddAsyncPreRules(IEnumerable<IAsyncRule<TIn>> rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            _preprocessingRules.AddRange(rules);
        }

        public void AddAsyncPostRules(IEnumerable<IAsyncRule<TOut>> rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            _postprocessingRules.AddRange(rules);
        }

        public void AddAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            _rules.AddRange(rules);
        }
    }

    public class AsyncRuleset<T>
    {
        private readonly List<IAsyncRule<T>> _rules;

        public AsyncRuleset()
        {
            _rules = new List<IAsyncRule<T>>();
        }

        public IEnumerable<IAsyncRule<T>> AsyncRules => _rules;

        public void AddAsyncRule(IAsyncRule<T> rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public void AddAsyncRules(IEnumerable<IAsyncRule<T>> rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            _rules.AddRange(rules);
        }
    }
}