using System.Collections.Generic;

namespace RulesEngine.Rules
{

    public class Ruleset<TIn, TOut>
    {
        private readonly List<IPreRule<TIn>> _preRules;

        private readonly List<IRule<TIn, TOut>> _rules;

        private readonly List<IPostRule<TOut>> _postRules;

        public Ruleset()
        {
            _preRules = new List<IPreRule<TIn>>();
            _rules = new List<IRule<TIn, TOut>>();
            _postRules = new List<IPostRule<TOut>>();            
        }

        public IEnumerable<IPreRule<TIn>> PreRules => _preRules;

        public IEnumerable<IRule<TIn, TOut>> Rules => _rules;

        public IEnumerable<IPostRule<TOut>> PostRules => _postRules;

        public virtual void AddPreRule(IPreRule<TIn> rule) {
            if (rule == null) throw new System.ArgumentNullException(nameof(rule));
            _preRules.Add(rule);
        }

        public virtual void AddPostRule(IPostRule<TOut> rule) {
            if (rule == null) throw new System.ArgumentNullException(nameof(rule));
            _postRules.Add(rule);
        }

        public virtual void AddRule(IRule<TIn, TOut> rule) {
            if (rule == null) throw new System.ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public virtual void AddPreRules(IEnumerable<IPreRule<TIn>> rules) {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            _preRules.AddRange(rules);
        }

        public virtual void AddPostRules(IEnumerable<IPostRule<TOut>> rules) {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            _postRules.AddRange(rules);
        }

        public virtual void AddRules(IEnumerable<IRule<TIn, TOut>> rules) {
            if (rules == null) throw new System.ArgumentNullException(nameof(rules));
            _rules.AddRange(rules);
        }
    }
}