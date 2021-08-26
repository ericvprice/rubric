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
            asyncPreRulesFull: ruleSet.AsyncPreRules,
            asyncRulesFull: ruleSet.AsyncRules,
            asyncPostRulesFull: ruleSet.AsyncPostRules,
            isParallel: isParallel,
            logger: logger)
        { }

        /// <summary>
        ///     Create a rule engine based on a synchronous ruleset.
        /// </summary>
        /// <param name="ruleSet">Collection of synchronous rules.</param>
        /// <param name="isParallel">Optionally execute in parallel.  False by default.</param>
        /// <param name="logger">An optional logger.</param>
        public AsyncRulesEngine(
            Ruleset<TIn, TOut> ruleSet,
            bool isParallel = false,
            ILogger logger = null
        ) : this(
            preRulesFull: ruleSet.PreRules, 
            rulesFull: ruleSet.Rules,
            postRulesFull: ruleSet.PostRules,
            isParallel: isParallel,
            logger: logger)
        { }

        /// <summary>
        ///     Construct an async rule engine based on inidvidual collections of async rules.
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
        ) : this(asyncPreRulesFull: asyncPreRules,
                 asyncRulesFull: asyncRules,
                 asyncPostRulesFull: asyncPostRules,
                 isParallel: isParallel,
                 logger: logger) { }


        /// <summary>
        ///     Construct an async rule engine based on inidvidual collections of async and sync rules.
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
            IEnumerable<IRule<TIn>> preRulesFull = null,
            IEnumerable<IAsyncRule<TIn>> asyncPreRulesFull = null,
            IEnumerable<IRule<TIn, TOut>> rulesFull = null,
            IEnumerable<IAsyncRule<TIn, TOut>> asyncRulesFull = null,
            IEnumerable<IRule<TOut>> postRulesFull = null,
            IEnumerable<IAsyncRule<TOut>> asyncPostRulesFull = null,
            bool isParallel = false,
            ILogger logger = null
        )
        {
            IsParallel = isParallel;
            preRulesFull ??= Enumerable.Empty<IRule<TIn>>();
            asyncPreRulesFull ??= Enumerable.Empty<IAsyncRule<TIn>>();
            rulesFull ??= Enumerable.Empty<IRule<TIn, TOut>>();
            asyncRulesFull ??= Enumerable.Empty<IAsyncRule<TIn, TOut>>();
            postRulesFull ??= Enumerable.Empty<IRule<TOut>>();
            asyncPostRulesFull ??= Enumerable.Empty<IAsyncRule<TOut>>();
            _preRules = preRulesFull.Select(r => r.WrapAsync())
                                .Concat(asyncPreRulesFull)
                                .ResolveDependencies()
                                .Select(e => e.ToArray())
                                .ToArray();
            _postRules = postRulesFull.Select(r => r.WrapAsync())
                                  .Concat(asyncPostRulesFull)
                                  .ResolveDependencies()
                                  .Select(e => e.ToArray())
                                  .ToArray();
            _rules = rulesFull.Select(r => r.WrapAsync())
                           .Concat(asyncRulesFull)
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
                    using (var logCtx = Logger.BeginScope(rule.Name))
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
                    using (var logCtx = Logger.BeginScope(rule.Name))
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
}