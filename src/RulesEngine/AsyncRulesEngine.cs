using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RulesEngine.Dependency;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace RulesEngine
{

    public class AsyncRulesEngine<TIn, TOut> : IAsyncRulesEngine<TIn, TOut>
    {

        /// <summary>
        ///     Ordered and parallelized pre processing rules
        /// </summary>
        private readonly IAsyncPreRule<TIn>[][] _preRules;

        /// <summary>
        ///     Ordered and parallelized postprocessing rules
        /// </summary>
        private readonly IAsyncPostRule<TOut>[][] _postRules;

        /// <summary>
        ///     Ordered and parallelized processing rules
        /// </summary>
        private readonly IAsyncRule<TIn, TOut>[][] _rules;

        /// <summary>
        ///     Convenience ruleset constructor.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            AsyncRuleset<TIn, TOut> ruleSet,
            ILogger logger = null
        ) : this(
            null,
            ruleSet.AsyncPreRules,
            null,
            ruleSet.AsyncRules,
            null,
            ruleSet.AsyncPostRules,
            logger)
        { }

        /// <summary>
        ///     Convenience ruleset constructor.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            Ruleset<TIn, TOut> ruleSet,
            ILogger logger = null
        ) : this(
            ruleSet.PreRules, null,
            ruleSet.Rules, null,
            ruleSet.PostRules, null,
            logger)
        { }

        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
        /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IAsyncPreRule<TIn>> asyncPreRules,
            IEnumerable<IAsyncRule<TIn, TOut>> asyncRules,
            IEnumerable<IAsyncPostRule<TOut>> asyncPostRules,
            ILogger logger = null
        ) : this(null, asyncPreRules, null, asyncRules, null, asyncPostRules, logger) { }


        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="preRules">Collection of synchronous preprocessing rules.</param>
        /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
        /// <param name="rules">Collection of synchronous processing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
        /// <param name="postRules">Collection of synchronous postprocessing rules.</param>
        /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IPreRule<TIn>> preRules,
            IEnumerable<IAsyncPreRule<TIn>> asyncPreRules,
            IEnumerable<IRule<TIn, TOut>> rules,
            IEnumerable<IAsyncRule<TIn, TOut>> asyncRules,
            IEnumerable<IPostRule<TOut>> postRules,
            IEnumerable<IAsyncPostRule<TOut>> asyncPostRules,
            ILogger logger = null
        )
        {
            _preRules =
                (preRules ?? Enumerable.Empty<IPreRule<TIn>>()).Select(r => r.WrapAsync())
                    .Concat(asyncPreRules ?? Enumerable.Empty<IAsyncPreRule<TIn>>())
                    .ResolveDependencies()
                    .Select(e => e.ToArray())
                    .ToArray();
            _postRules =
                (postRules ?? Enumerable.Empty<IPostRule<TOut>>()).Select(r => r.WrapAsync())
                    .Concat(asyncPostRules ?? Enumerable.Empty<IAsyncPostRule<TOut>>())
                    .ResolveDependencies()
                    .Select(e => e.ToArray())
                    .ToArray();
            _rules =
                (rules ?? Enumerable.Empty<IRule<TIn, TOut>>()).Select(r => r.WrapAsync())
                    .Concat(asyncRules ?? Enumerable.Empty<IAsyncRule<TIn, TOut>>())
                    .ResolveDependencies()
                    .Select(e => e.ToArray())
                    .ToArray();
            Logger = logger ?? NullLogger.Instance;
        }

        public bool ProcessInParallel { get; set; }

        public IEnumerable<IAsyncPreRule<TIn>> PreRules => _preRules.SelectMany(_ => _);

        public IEnumerable<IAsyncRule<TIn, TOut>> Rules => _rules.SelectMany(_ => _);

        public IEnumerable<IAsyncPostRule<TOut>> PostRules => _postRules.SelectMany(_ => _);

        public ILogger Logger { get; }

        private async Task ApplyPreRule(IEngineContext context, IAsyncPrePostRule<TIn> rule, TIn input)
        {
            try
            {
                var doesApply = await rule.DoesApply(context, input).ConfigureAwait(false);
                if (doesApply)
                {
                    Logger.LogTrace($"Rule {rule.Name} applies.");
                    Logger.LogTrace($"Applying {rule.Name}.");
                    await rule.Apply(context, input).ConfigureAwait(false);
                    Logger.LogTrace($"Finished applying {rule.Name}.");
                }
                else
                {
                    Logger.LogTrace($"Rule {rule.Name} does not apply.");
                }
            }
            catch (Exception e)
            {
                throw new EngineHaltException("Engine halted due to uncaught exception.", e)
                {
                    Context = context,
                    Input = input,
                    Output = null,
                    Rule = rule
                };
            }
        }

        private async Task ApplyPostRule(IEngineContext context, IAsyncPrePostRule<TOut> rule, TOut output)
        {
            try
            {
                var doesApply = await rule.DoesApply(context, output).ConfigureAwait(false);
                if (doesApply)
                {
                    Logger.LogTrace($"Rule {rule.Name} applies.");
                    Logger.LogTrace($"Applying {rule.Name}.");
                    await rule.Apply(context, output).ConfigureAwait(false);
                    Logger.LogTrace($"Finished applying {rule.Name}.");
                }
                else
                {
                    Logger.LogTrace($"Rule {rule.Name} does not apply.");
                }
            }
            catch (Exception e)
            {
                throw new EngineHaltException("Engine halted due to uncaught exception.", e)
                {
                    Context = context,
                    Input = null,
                    Output = output,
                    Rule = rule
                };
            }
        }

        private async Task ApplyRule(IEngineContext context, IAsyncRule<TIn, TOut> rule, TIn input, TOut output)
        {
            try
            {
                var doesApply = await rule.DoesApply(context, input, output).ConfigureAwait(false);
                if (doesApply)
                {
                    Logger.LogTrace($"Rule {rule.Name} applies.");
                    Logger.LogTrace($"Applying {rule.Name}.");
                    await rule.Apply(context, input, output).ConfigureAwait(false);
                    Logger.LogTrace($"Finished applying {rule.Name}.");
                }
                else
                {
                    Logger.LogTrace($"Rule {rule.Name} does not apply.");
                }
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

        public Task ApplyAsync(TIn input, TOut output, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext(Logger);
            return ProcessInParallel
                ? ApplyParallel(ctx, input, output)
                : ApplySerial(ctx, input, output);
        }

        private async Task ApplySerial(IEngineContext context, TIn input, TOut output)
        {
            foreach (var set in _preRules)
                foreach (var rule in set)
                    await ApplyPreRule(context, rule, input);
            foreach (var set in _rules)
                foreach (var rule in set)
                    await ApplyRule(context, rule, input, output);
            foreach (var set in _postRules)
                foreach (var rule in set)
                    await ApplyPostRule(context, rule, output);
        }

        private async Task ApplyParallel(IEngineContext context, TIn input, TOut output)
        {
            foreach (var set in _preRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPreRule(context, r, input)))
                ).ConfigureAwait(false);
            foreach (var set in _rules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyRule(context, r, input, output)))
                );
            foreach (var set in _postRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPostRule(context, r, output)))
                ).ConfigureAwait(false);
        }

        public Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext(Logger);
            return ProcessInParallel
                ? ApplyManyAsyncParallel(inputs, output, ctx)
                : ApplyManyAsyncSerial(inputs, output, ctx);
        }

        private async Task ApplyManyAsyncSerial(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            foreach (var input in inputs)
            {
                foreach (var set in _preRules)
                    foreach (var rule in set)
                        await ApplyPreRule(context, rule, input);
                foreach (var set in _rules)
                    foreach (var rule in set)
                        await ApplyRule(context, rule, input, output);
            }
            foreach (var set in _postRules)
                foreach (var rule in set)
                    await ApplyPostRule(context, rule, output);
        }

        private async Task ApplyManyAsyncParallel(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            foreach (var input in inputs)
            {
                foreach (var set in _preRules)
                    await Task.WhenAll(set.Select(r => Task.Run(() => ApplyPreRule(context, r, input))));
                foreach (var set in _rules)
                    await Task.WhenAll(set.Select(r => Task.Run(() => ApplyRule(context, r, input, output))));
            }
            foreach (var set in _postRules)
                await Task.WhenAll(set.Select(r => Task.Run(() => ApplyPostRule(context, r, output))));
        }

    }

}