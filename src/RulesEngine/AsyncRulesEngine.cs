using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine
{
    public class AsyncRulesEngine<TIn, TOut> : IAsyncRulesEngine<TIn, TOut>
        where TIn : class
        where TOut : class
    {
        /// <summary>
        ///     Ordered and parallelized postprocessing rules
        /// </summary>
        private readonly IAsyncRule<TOut>[][] _postRules;

        /// <summary>
        ///     Ordered and parallelized pre processing rules
        /// </summary>
        private readonly IAsyncRule<TIn>[][] _preRules;

        /// <summary>
        ///     Ordered and parallelized processing rules
        /// </summary>
        private readonly IAsyncRule<TIn, TOut>[][] _rules;

        /// <summary>
        ///     Convenience ruleset constructor.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            AsyncRuleset<TIn, TOut> ruleSet,
            bool isParallel = false,
            ILogger logger = null
        ) : this(
            null,
            ruleSet.AsyncPreRules,
            null,
            ruleSet.AsyncRules,
            null,
            ruleSet.AsyncPostRules,
            isParallel,
            logger)
        { }

        /// <summary>
        ///     Convenience ruleset constructor.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            Ruleset<TIn, TOut> ruleSet,
            bool isParallel = false,
            ILogger logger = null
        ) : this(
            ruleSet.PreRules, null,
            ruleSet.Rules, null,
            ruleSet.PostRules, null,
            isParallel,
            logger)
        { }

        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
        /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IAsyncRule<TIn>> asyncPreRules,
            IEnumerable<IAsyncRule<TIn, TOut>> asyncRules,
            IEnumerable<IAsyncRule<TOut>> asyncPostRules,
            ILogger logger = null,
            bool isParallel = false
        ) : this(null, asyncPreRules, null, asyncRules, null, asyncPostRules, isParallel, logger) { }


        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="preRules">Collection of synchronous preprocessing rules.</param>
        /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
        /// <param name="rules">Collection of synchronous processing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
        /// <param name="postRules">Collection of synchronous postprocessing rules.</param>
        /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IRule<TIn>> preRules,
            IEnumerable<IAsyncRule<TIn>> asyncPreRules,
            IEnumerable<IRule<TIn, TOut>> rules,
            IEnumerable<IAsyncRule<TIn, TOut>> asyncRules,
            IEnumerable<IRule<TOut>> postRules,
            IEnumerable<IAsyncRule<TOut>> asyncPostRules,
            bool isParallel = false,
            ILogger logger = null
        )
        {
            IsParallel = isParallel;
            _preRules =
                (preRules ?? Enumerable.Empty<IRule<TIn>>()).Select(r => r.WrapAsync())
                                                               .Concat(asyncPreRules ??
                                                                       Enumerable.Empty<IAsyncRule<TIn>>())
                                                               .ResolveDependencies()
                                                               .Select(e => e.ToArray())
                                                               .ToArray();
            _postRules =
                (postRules ?? Enumerable.Empty<IRule<TOut>>()).Select(r => r.WrapAsync())
                                                                  .Concat(asyncPostRules ??
                                                                          Enumerable.Empty<IAsyncRule<TOut>>())
                                                                  .ResolveDependencies()
                                                                  .Select(e => e.ToArray())
                                                                  .ToArray();
            _rules =
                (rules ?? Enumerable.Empty<IRule<TIn, TOut>>()).Select(r => r.WrapAsync())
                                                               .Concat(asyncRules ??
                                                                       Enumerable.Empty<IAsyncRule<TIn, TOut>>())
                                                               .ResolveDependencies()
                                                               .Select(e => e.ToArray())
                                                               .ToArray();
            Logger = logger ?? NullLogger.Instance;
        }

        public bool IsParallel { get; internal set; }

        public bool IsAsync => true;

        /// <inheritdoc />
        public Type InputType => typeof(TIn);

        /// <inheritdoc />
        public Type OutputType => typeof(TOut);

        public IEnumerable<IAsyncRule<TIn>> PreRules => _preRules.SelectMany(_ => _);

        public IEnumerable<IAsyncRule<TIn, TOut>> Rules => _rules.SelectMany(_ => _);

        public IEnumerable<IAsyncRule<TOut>> PostRules => _postRules.SelectMany(_ => _);

        public ILogger Logger { get; }

        public Task ApplyAsync(TIn input, TOut output, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext();
            SetupContext(ctx);
            return IsParallel
                ? ApplyParallel(ctx, input, output)
                : ApplySerial(ctx, input, output);
        }

        public Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext();
            SetupContext(ctx);
            return IsParallel
                ? ApplyManyAsyncParallel(inputs, output, ctx)
                : ApplyManyAsyncSerial(inputs, output, ctx);
        }

        private async Task ApplyPrePostRule<T>(IEngineContext context, IAsyncRule<T> rule, T input) where T : class
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
                    Input = input is TIn @in ? @in : default,
                    Output = input is TOut @out ? @out : default,
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

        private async Task ApplySerial(IEngineContext context, TIn input, TOut output)
        {
            foreach (var set in _preRules)
                foreach (var rule in set)
                    await ApplyPrePostRule(context, rule, input).ConfigureAwait(false);
            foreach (var set in _rules)
                foreach (var rule in set)
                    await ApplyRule(context, rule, input, output).ConfigureAwait(false);
            foreach (var set in _postRules)
                foreach (var rule in set)
                    await ApplyPrePostRule(context, rule, output).ConfigureAwait(false);
        }

        private async Task ApplyParallel(IEngineContext context, TIn input, TOut output)
        {
            foreach (var set in _preRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, input)))
                ).ConfigureAwait(false);
            foreach (var set in _rules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyRule(context, r, input, output)))
                ).ConfigureAwait(false);
            foreach (var set in _postRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, output)))
                ).ConfigureAwait(false);
        }

        private async Task ApplyManyAsyncSerial(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            foreach (var input in inputs)
            {
                foreach (var set in _preRules)
                    foreach (var rule in set)
                        await ApplyPrePostRule(context, rule, input).ConfigureAwait(false);
                foreach (var set in _rules)
                    foreach (var rule in set)
                        await ApplyRule(context, rule, input, output).ConfigureAwait(false);
            }

            foreach (var set in _postRules)
                foreach (var rule in set)
                    await ApplyPrePostRule(context, rule, output);
        }

        private async Task ApplyManyAsyncParallel(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            foreach (var input in inputs)
            {
                foreach (var set in _preRules)
                    await Task.WhenAll(
                        set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, input)))
                    ).ConfigureAwait(false);
                foreach (var set in _rules)
                    await Task.WhenAll(
                        set.Select(r => Task.Run(() => ApplyRule(context, r, input, output)))
                    ).ConfigureAwait(false);
            }
            foreach (var set in _postRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, output)))
                ).ConfigureAwait(false);
        }

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;
    }

    public class AsyncRulesEngine<T> : IAsyncRulesEngine<T>
        where T : class
    {
        /// <summary>
        ///     Ordered and parallelized processing rules
        /// </summary>
        private readonly IAsyncRule<T>[][] _rules;

        /// <summary>
        ///     Convenience ruleset constructor.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            AsyncRuleset<T> ruleSet,
            bool isParallel = false,
            ILogger logger = null
        ) : this(
            null,
            ruleSet.AsyncRules,
            isParallel,
            logger)
        { }

        /// <summary>
        ///     Convenience ruleset constructor.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous and synchronous rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            Ruleset<T> ruleSet,
            bool isParallel = false,
            ILogger logger = null
        ) : this(
            ruleSet.Rules, null,
            isParallel,
            logger)
        { }

        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
        /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IAsyncRule<T>> asyncRules,
            ILogger logger = null,
            bool isParallel = false
        ) : this(null, asyncRules, isParallel, logger) { }


        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="preRules">Collection of synchronous preprocessing rules.</param>
        /// <param name="asyncPreRules">Collection of asynchronous preprocessing rules.</param>
        /// <param name="rules">Collection of synchronous processing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
        /// <param name="postRules">Collection of synchronous postprocessing rules.</param>
        /// <param name="asyncPostRules">Collection of asynchronous postprocessing rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IRule<T>> rules,
            IEnumerable<IAsyncRule<T>> asyncRules,
            bool isParallel = false,
            ILogger logger = null
        )
        {
            IsParallel = isParallel;
            _rules =
                (rules ?? Enumerable.Empty<IRule<T>>()).Select(r => r.WrapAsync())
                                                               .Concat(asyncRules ??
                                                                       Enumerable.Empty<IAsyncRule<T>>())
                                                               .ResolveDependencies()
                                                               .Select(e => e.ToArray())
                                                               .ToArray();
            Logger = logger ?? NullLogger.Instance;
        }

        public bool IsParallel { get; internal set; }

        public bool IsAsync => true;

        /// <inheritdoc />
        public Type InputType => typeof(T);

        /// <inheritdoc />
        public Type OutputType => typeof(T);

        public IEnumerable<IAsyncRule<T>> Rules => _rules.SelectMany(_ => _);

        public ILogger Logger { get; }

        public Task ApplyAsync(T input, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext();
            SetupContext(ctx);
            return IsParallel
                ? ApplyParallel(ctx, input)
                : ApplySerial(ctx, input);
        }

        public Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null)
        {
            var ctx = context ?? new EngineContext();
            SetupContext(ctx);
            return IsParallel
                ? ApplyManyAsyncParallel(inputs, ctx)
                : ApplyManyAsyncSerial(inputs, ctx);
        }

        private async Task ApplyRule(IEngineContext context, IAsyncRule<T> rule, T input)
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

        private async Task ApplySerial(IEngineContext context, T input)
        {
            foreach (var set in _rules)
                foreach (var rule in set)
                    await ApplyRule(context, rule, input).ConfigureAwait(false);
        }

        private async Task ApplyParallel(IEngineContext context, T input)
        {
            foreach (var set in _rules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyRule(context, r, input)))
                ).ConfigureAwait(false);
        }

        private async Task ApplyManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context)
        {
            foreach (var input in inputs)
            {
                foreach (var set in _rules)
                    foreach (var rule in set)
                        await ApplyRule(context, rule, input).ConfigureAwait(false);
            }
        }

        private async Task ApplyManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context)
        {
            foreach (var input in inputs)
            {
                foreach (var set in _rules)
                    await Task.WhenAll(
                        set.Select(r => Task.Run(() => ApplyRule(context, r, input)))
                    ).ConfigureAwait(false);
            }
        }

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;
    }
}