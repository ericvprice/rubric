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
        where TIn : class
        where TOut : class
    {
        private readonly IRule<TOut>[][] _postprocessingRules;

        private readonly IRule<TIn>[][] _preprocessingRules;

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
            IEnumerable<IRule<TIn>> preprocessingRules,
            IEnumerable<IRule<TIn, TOut>> rules,
            IEnumerable<IRule<TOut>> postprocessingRules,
            ILogger logger = null
        )
        {
            _preprocessingRules =
                (preprocessingRules ?? Enumerable.Empty<IRule<TIn>>())
                .ResolveDependencies()
                .Select(e => e.ToArray())
                .ToArray();
            _postprocessingRules =
                (postprocessingRules ?? Enumerable.Empty<IRule<TOut>>())
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
            var ctx = context ?? new EngineContext();
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
            var ctx = context ?? new EngineContext();
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

        public IEnumerable<IRule<TIn>> PreRules
            => _preprocessingRules.SelectMany(_ => _);

        public IEnumerable<IRule<TIn, TOut>> Rules
            => _rules.SelectMany(_ => _);

        public IEnumerable<IRule<TOut>> PostRules
            => _postprocessingRules.SelectMany(_ => _);

        /// <inheritdoc />
        public ILogger Logger { get; }

        /// <inheritdoc />
        public bool IsAsync => false;

        /// <inheritdoc />
        public bool IsParallel => false;

        /// <inheritdoc />
        public Type InputType => typeof(TIn);

        /// <inheritdoc />
        public Type OutputType => typeof(TOut);

        private void ApplyPrePostRule<T>(IEngineContext context, IRule<T> rule, T input)
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
                throw new EngineExecutionException("Engine halted due to uncaught exception.", e)
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
                throw new EngineExecutionException("Engine halted due to uncaught exception.", e)
                {
                    Context = context,
                    Input = input,
                    Output = output,
                    Rule = rule
                };
            }
        }

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;

    }
}