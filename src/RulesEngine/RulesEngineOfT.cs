using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine
{
    public class RulesEngine<T> : IRulesEngine<T>
        where T : class
    {
        private readonly IRule<T>[][] _rules;

        public RulesEngine(Ruleset<T> ruleset, ILogger logger = null)
            : this(ruleset.Rules, logger) { }

        /// <summary>
        ///     Default public constructor.
        /// </summary>
        /// <param name="preprocessingRules">Collection of synchronous preprocessing rules.</param>
        /// <param name="rules">Collection of synchronous processing rules.</param>
        /// <param name="postprocessingRules">Collection of synchronous postprocessing rules.</param>
        /// <param name="logger">An optional logger.</param>
        public RulesEngine(
            IEnumerable<IRule<T>> rules,
            ILogger logger = null
        )
        {
            _rules =
                (rules ?? Enumerable.Empty<IRule<T>>())
                .ResolveDependencies()
                .Select(e => e.ToArray())
                .ToArray();
            Logger = logger ?? NullLogger.Instance;
        }

        public void Apply(T input, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext();
            SetupContext(ctx);
            foreach (var set in _rules)
                foreach (var rule in set)
                    ApplyRule(ctx, rule, input);
        }


        public void Apply(IEnumerable<T> inputs, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext();
            foreach (var input in inputs)
                foreach (var set in _rules)
                    foreach (var rule in set)
                        ApplyRule(ctx, rule, input);
        }

        public IEnumerable<IRule<T>> Rules
            => _rules.SelectMany(_ => _);

        /// <inheritdoc />
        public ILogger Logger { get; }

        /// <inheritdoc />
        public bool IsAsync => false;

        /// <inheritdoc />
        public bool IsParallel => false;

        /// <inheritdoc />
        public Type InputType => typeof(T);

        /// <inheritdoc />
        public Type OutputType => typeof(T);

        private void ApplyRule(IEngineContext context, IRule<T> rule, T input)
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
                    Input = input,
                    Rule = rule
                };
            }
        }

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;
    }
}