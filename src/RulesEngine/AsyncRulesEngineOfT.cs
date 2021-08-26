using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RulesEngine.Dependency;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine
{
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
            bool isParallel = false,
            ILogger logger = null
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
                ? ApplyParallel(input, ctx)
                : ApplySerial(input, ctx);
        }

        public Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null, bool parallelizeInputs = false)
        {
            var ctx = context ?? new EngineContext();
            SetupContext(ctx);
            if (IsParallel)
                if (parallelizeInputs)
                    return ApplyParallelManyAsyncParallel(inputs, ctx);
                else
                    return ApplyManyAsyncParallel(inputs, ctx);
            else
                if (parallelizeInputs)
                return ApplyParallelManyAsyncSerial(inputs, ctx);
            else
                return ApplyManyAsyncSerial(inputs, ctx);
        }

        private async Task ApplyRule(IEngineContext context, IAsyncRule<T> rule, T input)
        {
            try
            {
                var doesApply = await rule.DoesApply(context, input).ConfigureAwait(false);
                if (doesApply)
                {
                    using (var logCtx = Logger.BeginScope(rule.Name))
                    {
                        Logger.LogTrace($"Rule {rule.Name} applies.");
                        Logger.LogTrace($"Applying {rule.Name}.");
                        await rule.Apply(context, input).ConfigureAwait(false);
                        Logger.LogTrace($"Finished applying {rule.Name}.");
                    }
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

        private async Task ApplySerial(T input, IEngineContext context)
        {
            foreach (var set in _rules)
                foreach (var rule in set)
                    await ApplyRule(context, rule, input).ConfigureAwait(false);
        }

        private async Task ApplyParallel(T input, IEngineContext context)
        {
            foreach (var set in _rules)
                await Task.WhenAll(
                    set.Select(r => Task.Run(() => ApplyRule(context, r, input)))
                ).ConfigureAwait(false);
        }

        private async Task ApplyManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context)
        {
            foreach (var input in inputs) await ApplyAsync(input, context).ConfigureAwait(false);
        }

        private Task ApplyParallelManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context)
            => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplySerial(i, context))));

        private async Task ApplyManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context)
        {
            foreach (var input in inputs) await ApplyParallel(input, context).ConfigureAwait(false);
        }

        private Task ApplyParallelManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context)
            => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyParallel(i, context))));

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;
    }
}