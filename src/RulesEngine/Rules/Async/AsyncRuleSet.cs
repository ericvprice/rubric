using System.Collections.Generic;

namespace RulesEngine.Rules.Async
{
    public class AsyncRuleset<TIn, TOut>
    {
        private readonly List<IAsyncPreRule<TIn>> _preprocessingRules;

        private readonly List<IAsyncRule<TIn, TOut>> _rules;

        private readonly List<IAsyncPostRule<TOut>> _postprocessingRules;

        public AsyncRuleset()
        {
            _preprocessingRules = new List<IAsyncPreRule<TIn>>();
            _rules = new List<IAsyncRule<TIn, TOut>>();
            _postprocessingRules = new List<IAsyncPostRule<TOut>>();
        }

        public IEnumerable<IAsyncPreRule<TIn>> AsyncPreRules => _preprocessingRules;

        public IEnumerable<IAsyncRule<TIn, TOut>> AsyncRules => _rules;

        public IEnumerable<IAsyncPostRule<TOut>> AsyncPostRules => _postprocessingRules;

        public void AddAsyncPreRule(IAsyncPreRule<TIn> rule)
        {
            if (rule == null) throw new System.ArgumentNullException(nameof(rule));
            _preprocessingRules.Add(rule);
        }

        public void AddAsyncPostRule(IAsyncPostRule<TOut> rule)
        {
            if (rule == null) throw new System.ArgumentNullException(nameof(rule));
            _postprocessingRules.Add(rule);
        }

        public void AddAsyncRule(IAsyncRule<TIn, TOut> rule)
        {
            if (rule == null) throw new System.ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public void AddAsyncPreRules(IEnumerable<IAsyncPreRule<TIn>> rules)
        {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            _preprocessingRules.AddRange(rules);
        }

        public void AddAsyncPostRules(IEnumerable<IAsyncPostRule<TOut>> rules)
        {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            _postprocessingRules.AddRange(rules);
        }

        public void AddAsyncRules(IEnumerable<IAsyncRule<TIn, TOut>> rules)
        {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            _rules.AddRange(rules);
        }
    }
}