using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine
{
    public class RulesEngine<TIn, TOut> : IRulesEngine<TIn, TOut>
    {
        private readonly IPostRule<TOut>[][] _postprocessingRules;

        private readonly IPreRule<TIn>[][] _preprocessingRules;

        private readonly IRule<TIn, TOut>[][] _rules;

        public RulesEngine(Ruleset<TIn, TOut> ruleset, ILogger logger = null)
            : this(ruleset.PreRules, ruleset.Rules, ruleset.PostRules, logger) { }

        /// <summary>
        ///     Default public constructor.
        /// </summary>
        /// <param name="preprocessingRules">Collection of synchronous preprocessing rules.</param>
        /// <param name="rules">Collection of synchronous processing rules.</param>
        /// <param name="postprocessingRules">Collection of synchronous postprocessing rules.</param>
        /// <param name="logger">An optional logger.</param>
        public RulesEngine(
            IEnumerable<IPreRule<TIn>> preprocessingRules,
            IEnumerable<IRule<TIn, TOut>> rules,
            IEnumerable<IPostRule<TOut>> postprocessingRules,
            ILogger logger = null
        )
        {
            _preprocessingRules =
                (preprocessingRules ?? Enumerable.Empty<IPreRule<TIn>>())
                .ResolveDependencies()
                .Select(e => e.ToArray())
                .ToArray();
            _postprocessingRules =
                (postprocessingRules ?? Enumerable.Empty<IPostRule<TOut>>())
                .ResolveDependencies()
                .Select(e => e.ToArray())
                .ToArray();
            _rules =
                (rules ?? Enumerable.Empty<IRule<TIn, TOut>>())
                .ResolveDependencies()
                .Select(e => e.ToArray())
                .ToArray();
            Logger = logger ?? NullLogger.Instance;
        }

        public void Apply(TIn input, TOut output, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext(Logger);
            SetupContext(ctx);
            foreach (var set in _preprocessingRules)
                foreach (var rule in set)
                    ApplyPrePostRule(ctx, rule, input);
            foreach (var set in _rules)
                foreach (var rule in set)
                    ApplyRule(ctx, rule, input, output);
            foreach (var set in _postprocessingRules)
                foreach (var rule in set)
                    ApplyPrePostRule(ctx, rule, output);
        }


        public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext(Logger);
            foreach (var input in inputs)
            {
                foreach (var set in _preprocessingRules)
                    foreach (var rule in set)
                        ApplyPrePostRule(ctx, rule, input);
                foreach (var set in _rules)
                    foreach (var rule in set)
                        ApplyRule(ctx, rule, input, output);
            }

            foreach (var set in _postprocessingRules)
                foreach (var rule in set)
                    ApplyPrePostRule(ctx, rule, output);
        }

        public IEnumerable<IPreRule<TIn>> PreRules
            => _preprocessingRules.SelectMany(_ => _);

        public IEnumerable<IRule<TIn, TOut>> Rules
            => _rules.SelectMany(_ => _);

        public IEnumerable<IPostRule<TOut>> PostRules
            => _postprocessingRules.SelectMany(_ => _);

        public ILogger Logger { get; }

        public bool IsAsync => false;

        public bool IsParallel => false;
        
        private void ApplyPrePostRule<T>(IEngineContext context, IPrePostRule<T> rule, T input)
        {
            try
            {
                var doesApply = rule.DoesApply(context, input);
                Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
                if (!doesApply) return;
                Logger.LogTrace($"Applying {rule.Name}.");
                rule.Apply(context, input);
                Logger.LogTrace($"Finished applying {rule.Name}.");
            }
            catch (Exception e)
            {
                throw new EngineHaltException("Engine halted due to uncaught exception.", e)
                {
                    Context = context,
                    Input = input is TIn @in ? @in : default,
                    Output = input is TOut @out ? @out : default,
                    Rule = rule
                };
            }
        }

        private void ApplyRule(IEngineContext context, IRule<TIn, TOut> rule, TIn input, TOut output)
        {
            try
            {
                var doesApply = rule.DoesApply(context, input, output);
                Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
                if (!doesApply) return;
                Logger.LogTrace($"Applying {rule.Name}.");
                rule.Apply(context, input, output);
                Logger.LogTrace($"Finished applying {rule.Name}.");
            }
            catch (Exception e)
            {
                throw new EngineHaltException("Engine halted due to uncaught exception.", e)
                {
                    Context = context,
                    Input = input,
                    Output = output,
                    Rule = rule
                };
            }
        }

        private void SetupContext(IEngineContext ctx) => ctx[EngineConextExtensions.ENGINE_KEY] = this;
    }
}