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
        ///     Create an engine with the given rules.h
        /// </summary>
        /// <param name="rules">Collection of asynchronous processing rules.</param>
        /// <param name="isParallel">Whether to execute rules in parallel.</param>
        /// <param name="logger">A logger.</param>
        public AsyncRulesEngine(
            IEnumerable<IAsyncRule<T>> rules,
            bool isParallel = false,
            ILogger logger = null
        ) : this(null, rules, isParallel, logger) { }


        /// <summary>
        ///     Full constructor.
        /// </summary>
        /// <param name="rules">Collection of synchronous processing rules.</param>
        /// <param name="asyncRules">Collection of asynchronous processing rules.</param>
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

        public Task ApplyAsync(T input) => ApplyAsync(input, new EngineContext());

        public Task ApplyAsync(T input, IEngineContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            SetupContext(context);
            return IsParallel
                ? ApplyParallel(input, context)
                : ApplySerial(input, context);
        }

        public Task ApplyAsync(IEnumerable<T> inputs, bool parallelizeInputs = false)
            => ApplyAsync(inputs, new EngineContext(), parallelizeInputs);

        public Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context, bool parallelizeInputs = false)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            SetupContext(context);
            if (IsParallel)
                if (parallelizeInputs)
                    return ApplyParallelManyAsyncParallel(inputs, context);
                else
                    return ApplyManyAsyncParallel(inputs, context);
            else
                if (parallelizeInputs)
                return ApplyParallelManyAsyncSerial(inputs, context);
            else
                return ApplyManyAsyncSerial(inputs, context);
        }

        private async Task ApplyRule(IEngineContext context, IAsyncRule<T> rule, T input, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                using (Logger.BeginScope(rule.Name))
                {
                    if (await rule.DoesApply(context, input, cancellationTokenSource.Token).ConfigureAwait(false))
                    {
                        {
                            Logger.LogTrace($"Rule {rule.Name} applies.");
                            Logger.LogTrace($"Applying {rule.Name}.");
                            await rule.Apply(context, input, cancellationTokenSource.Token).ConfigureAwait(false);
                            Logger.LogTrace($"Finished applying {rule.Name}.");
                        }
                    }
                    else
                    {
                        Logger.LogTrace($"Rule {rule.Name} does not apply.");
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    throw new EngineExecutionException("Engine halted due uncaught exception: ", oce);
                }
                //Otherwise, do nothing... this is expected
            }
            catch (EngineHaltException)
            {
                //Cancel and throw.
                cancellationTokenSource.Cancel();
                throw;
            }
            catch (Exception e)
            {
                //Wrap all other exceptions and throw as engine execution exception and cancel other tasks
                cancellationTokenSource.Cancel();
                throw new EngineExecutionException("Engine halted due to uncaught exception.", e)
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
            var cancellationTokenSource = new CancellationTokenSource();
            try
            {
                foreach (var set in _rules)
                    foreach (var rule in set)
                        await ApplyRule(context, rule, input, cancellationTokenSource).ConfigureAwait(false);
            }
            catch (EngineHaltException)
            {
                //Do nothing
            }
        }

        private async Task ApplyParallel(T input, IEngineContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            foreach (var set in _rules)
                try
                {
                    await Task.WhenAll(
                        set.Select(r => Task.Run(() => ApplyRule(context, r, input, cancellationTokenSource), cancellationTokenSource.Token))
                    ).ConfigureAwait(false);
                }
                catch (EngineHaltException)
                {
                    cancellationTokenSource.Cancel();
                    return;
                }
                catch (EngineExecutionException)
                {
                    cancellationTokenSource.Cancel();
                    throw;
                }
        }

        private async Task ApplyManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context)
        {
            foreach (var input in inputs)
                await ApplyAsync(input, context).ConfigureAwait(false);
        }

        private Task ApplyParallelManyAsyncSerial(IEnumerable<T> inputs, IEngineContext context)
            => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplySerial(i, context))));

        private async Task ApplyManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context)
        {
            foreach (var input in inputs)
                await ApplyParallel(input, context).ConfigureAwait(false);
        }

        private Task ApplyParallelManyAsyncParallel(IEnumerable<T> inputs, IEngineContext context)
            => Task.WhenAll(inputs.Select(i => Task.Run(() => ApplyParallel(i, context))));

        internal void SetupContext(IEngineContext ctx) => ctx[EngineContextExtensions.ENGINE_KEY] = this;
    }
}