using Microsoft.Extensions.Logging;
using Rubric.Engines.Implementation;
using Rubric.Engines.Probabilistic.Implementation;
using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Engines.Probabilistic;

internal static class ProbabilisiticEngineExtensions
{
  public const string RandomKey = "__RANDOM";

  private const string DoesNotApply = "Rule {Name} does not apply.";
  private const string Applies = "Rule {Name} does not applies.";
  private const string Applying = "Applying {Name}.";
  private const string Done = "Finished applying {Name}.";

  private static readonly Action<ILogger, string, Exception> _doesNotApplyLogger
    = LoggerMessage.Define<string>(LogLevel.Trace, new(1, "Rule does not apply"), DoesNotApply);

  private static readonly Action<ILogger, string, Exception> _appliesLogger
    = LoggerMessage.Define<string>(LogLevel.Trace, new(2, "Rule applies"), Applies);

  private static readonly Action<ILogger, string, Exception> _applyingLogger
    = LoggerMessage.Define<string>(LogLevel.Trace, new(1, "Appling rule"), Applying);

  private static readonly Action<ILogger, string, Exception> _doneLogger
    = LoggerMessage.Define<string>(LogLevel.Trace, new(1, "Rule applied"), Done);

  /// <summary>
  ///   Apply an async preprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPreRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    IRule<T> r,
    T i,
    CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      var task = r.CacheBehavior.Behavior switch
      {
        CacheBehavior.PerInput => ctx.GetInputPredicateCache()
                                     .GetOrAddAsync(r.CacheBehavior.Key,
                                                    async _ => e.Random.NextDouble()
                                                               < await r.DoesApply(ctx, i, t).ConfigureAwait(false)),
        CacheBehavior.PerExecution => ctx.GetExecutionPredicateCache()
                                         .GetOrAddAsync(r.CacheBehavior.Key,
                                                        async _ => e.Random.NextDouble()
                                                                   < await r.DoesApply(ctx, i, t).ConfigureAwait(false)),
        _ => Task.FromResult(e.Random.NextDouble() < await r.DoesApply(ctx, i, t).ConfigureAwait(false))
      };
      var result = await task.ConfigureAwait(false);
      if (!result)
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
        return;
      }
      _appliesLogger(e.Logger, r.Name, null);
      _applyingLogger(e.Logger, r.Name, null);
      await r.Apply(ctx, i, t).ConfigureAwait(false);
      _doneLogger(e.Logger, r.Name, null);
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, null, t)) throw;
    }
  }

  /// <summary>
  ///   Apply an async postprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPostRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    IRule<T> r,
    T o,
    CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      var task = r.CacheBehavior.Behavior switch
      {
        CacheBehavior.PerExecution => ctx.GetExecutionPredicateCache()
                                         .GetOrAddAsync(r.CacheBehavior.Key,
                                                        async _ => e.Random.NextDouble()
                                                                   < await r.DoesApply(ctx, o, t).ConfigureAwait(false)),
        _ => Task.FromResult(e.Random.NextDouble() < await r.DoesApply(ctx, o, t).ConfigureAwait(false))
      };
      var result = await task.ConfigureAwait(false);
      if (!result)
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
        return;
      }
      _appliesLogger(e.Logger, r.Name, null);
      _applyingLogger(e.Logger, r.Name, null);
      await r.Apply(ctx, o, t).ConfigureAwait(false);
      _doneLogger(e.Logger, r.Name, null);
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, null, o, t)) throw;
    }
  }

  /// <summary>
  ///   Apply an async rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="o">The o item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncRule<TIn, TOut>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    IRule<TIn, TOut> r,
    TIn i,
    TOut o,
    CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      var task = r.CacheBehavior.Behavior switch
      {
        CacheBehavior.PerInput => ctx.GetInputPredicateCache()
                                     .GetOrAddAsync(r.CacheBehavior.Key, 
                                                    async _ => e.Random.NextDouble() 
                                                               < await r.DoesApply(ctx, i, o, t).ConfigureAwait(false)),
        CacheBehavior.PerExecution => ctx.GetExecutionPredicateCache()
                                         .GetOrAddAsync(r.CacheBehavior.Key, 
                                                        async _ => e.Random.NextDouble() 
                                                                   < await r.DoesApply(ctx, i, o, t).ConfigureAwait(false)),
        _ => Task.FromResult(e.Random.NextDouble() < await r.DoesApply(ctx, i, o, t).ConfigureAwait(false))
      };
      var result = await task.ConfigureAwait(false);
      if (!result)
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
        return;
      }
      using var logCtx = e.Logger.BeginScope(r.Name);
      _appliesLogger(e.Logger, r.Name, null);
      _applyingLogger(e.Logger, r.Name, null);
      await r.Apply(ctx, i, o, t).ConfigureAwait(false);
      _doneLogger(e.Logger, r.Name, null);
    
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, o, t)) throw;
    }
  }

  /// <summary>
  ///   Apply a pre-rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  internal static void ApplyPreRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx, Rules.Probabilistic.IRule<T> r,
    T i)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      var result = r.CacheBehavior.Behavior switch
      {
        CacheBehavior.PerInput => ctx.GetInputPredicateCache()
                                     .GetOrAdd(r.CacheBehavior.Key, _ => e.Random.NextDouble() < r.DoesApply(ctx, i)),
        CacheBehavior.PerExecution => ctx.GetExecutionPredicateCache()
                                         .GetOrAdd(r.CacheBehavior.Key, _ => e.Random.NextDouble() < r.DoesApply(ctx, i)),
        _ => e.Random.NextDouble() < r.DoesApply(ctx, i)
      };
      if (result)
      {
        _appliesLogger(e.Logger, r.Name, null);
        _applyingLogger(e.Logger, r.Name, null);
        r.Apply(ctx, i);
        _doneLogger(e.Logger, r.Name, null);
      }
      else
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
      }
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, null)) throw;
    }
  }

  /// <summary>
  ///   Apply a rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">Engine.</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="o">The current o item.</param>
  internal static void ApplyRule<TIn, TOut>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx, Rules.Probabilistic.IRule<TIn, TOut> r,
    TIn i,
    TOut o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      var result = r.CacheBehavior.Behavior switch
      {
        CacheBehavior.PerInput => ctx.GetInputPredicateCache()
                                     .GetOrAdd(r.CacheBehavior.Key, _ => e.Random.NextDouble() < r.DoesApply(ctx, i, o)),
        CacheBehavior.PerExecution => ctx.GetExecutionPredicateCache()
                                         .GetOrAdd(r.CacheBehavior.Key, _ => e.Random.NextDouble() < r.DoesApply(ctx, i, o)),
        _ => e.Random.NextDouble() < r.DoesApply(ctx, i, o)
      };
      if (result)
      {
        _appliesLogger(e.Logger, r.Name, null);
        _applyingLogger(e.Logger, r.Name, null);
        r.Apply(ctx, i, o);
        _doneLogger(e.Logger, r.Name, null);
      }
      else
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
      }
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, o)) throw;
    }
  }

  /// <summary>
  ///   Apply a postprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  internal static void ApplyPostRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx, Rules.Probabilistic.IRule<T> r,
    T o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      var result = r.CacheBehavior.Behavior switch
      {
        CacheBehavior.PerInput => ctx.GetInputPredicateCache()
                                     .GetOrAdd(r.CacheBehavior.Key, _ => e.Random.NextDouble() < r.DoesApply(ctx, o)),
        CacheBehavior.PerExecution => ctx.GetExecutionPredicateCache()
                                         .GetOrAdd(r.CacheBehavior.Key, _ => e.Random.NextDouble() < r.DoesApply(ctx, o)),
        _ => e.Random.NextDouble() < r.DoesApply(ctx, o)
      };
      if (result)
      {
        _appliesLogger(e.Logger, r.Name, null);
        _applyingLogger(e.Logger, r.Name, null);
        r.Apply(ctx, o);
        _doneLogger(e.Logger, r.Name, null);
      }
      else
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
      }
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, null, o)) throw;
    }
  }
}