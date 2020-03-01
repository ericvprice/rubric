using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task ApplyAsync(TIn input, TOut output) => ApplyAsync(input, output, new EngineContext());

        public Task ApplyAsync(TIn input, TOut output, IEngineContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            SetupContext(context);
            return IsParallel
                ? ApplyParallel(context, input, output)
                : ApplySerial(context, input, output);
        }

        public Task ApplyAsync(IEnumerable<TIn> inputs, TOut output)
            => ApplyAsync(inputs, output, new EngineContext());


        public Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            SetupContext(context);
            return IsParallel
                ? ApplyManyAsyncParallel(inputs, output, context)
                : ApplyManyAsyncSerial(inputs, output, context);
        }

        private async Task ApplyPrePostRule<T>(IEngineContext context, IAsyncRule<T> rule, T input, CancellationTokenSource cts) where T : class
        {
            try
            {
                var doesApply = await rule.DoesApply(context, input, cts.Token).ConfigureAwait(false);
                if (doesApply)
                {
                    Logger.LogTrace($"Rule {rule.Name} applies.");
                    Logger.LogTrace($"Applying {rule.Name}.");
                    using (var logCtx = Logger.BeginScope(rule.Name))
                        await rule.Apply(context, input, cts.Token).ConfigureAwait(false);
                    Logger.LogTrace($"Finished applying {rule.Name}.");
                }
                else
                {
                    Logger.LogTrace($"Rule {rule.Name} does not apply.");
                }
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

        private async Task ApplyRule(IEngineContext context, IAsyncRule<TIn, TOut> rule, TIn input, TOut output, CancellationTokenSource cts)
        {
            try
            {
                var doesApply = await rule.DoesApply(context, input, output, cts.Token).ConfigureAwait(false);
                if (doesApply)
                {
                    Logger.LogTrace($"Rule {rule.Name} applies.");
                    Logger.LogTrace($"Applying {rule.Name}.");
                    using (var logCtx = Logger.BeginScope(rule.Name))
                        await rule.Apply(context, input, output, cts.Token).ConfigureAwait(false);
                    Logger.LogTrace($"Finished applying {rule.Name}.");
                }
                else
                {
                    Logger.LogTrace($"Rule {rule.Name} does not apply.");
                }
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

        private async Task ApplySerial(IEngineContext context, TIn input, TOut output)
        {
            var tokenSource = new CancellationTokenSource();
            foreach (var set in _preRules)
                foreach (var rule in set)
                    await ApplyPrePostRule(context, rule, input, tokenSource).ConfigureAwait(false);
            foreach (var set in _rules)
                foreach (var rule in set)
                    await ApplyRule(context, rule, input, output, tokenSource).ConfigureAwait(false);
            foreach (var set in _postRules)
                foreach (var rule in set)
                    await ApplyPrePostRule(context, rule, output, tokenSource).ConfigureAwait(false);
        }

        private async Task ApplyParallel(IEngineContext context, TIn input, TOut output)
        {
            var tokenSource = new CancellationTokenSource();
            foreach (var set in _preRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, input, tokenSource)))
                ).ConfigureAwait(false);
            foreach (var set in _rules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyRule(context, r, input, output, tokenSource)))
                ).ConfigureAwait(false);
            foreach (var set in _postRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, output, tokenSource)))
                ).ConfigureAwait(false);
        }

        private async Task ApplyManyAsyncSerial(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            var tokenSource = new CancellationTokenSource();
            foreach (var input in inputs)
            {
                foreach (var set in _preRules)
                    foreach (var rule in set)
                        await ApplyPrePostRule(context, rule, input, tokenSource).ConfigureAwait(false);
                foreach (var set in _rules)
                    foreach (var rule in set)
                        await ApplyRule(context, rule, input, output, tokenSource).ConfigureAwait(false);
            }

            foreach (var set in _postRules)
                foreach (var rule in set)
                    await ApplyPrePostRule(context, rule, output, tokenSource);
        }

        private async Task ApplyManyAsyncParallel(IEnumerable<TIn> inputs, TOut output, IEngineContext context)
        {
            var tokenSource = new CancellationTokenSource();
            foreach (var input in inputs)
            {
                foreach (var set in _preRules)
                    await Task.WhenAll(
                        set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, input, tokenSource)))
                    ).ConfigureAwait(false);
                foreach (var set in _rules)
                    await Task.WhenAll(
                        set.Select(r => Task.Run(() => ApplyRule(context, r, input, output, tokenSource)))
                    ).ConfigureAwait(false);
            }
            foreach (var set in _postRules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyPrePostRule(context, r, output, tokenSource)))
                ).ConfigureAwait(false);
        }

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;

    }
}