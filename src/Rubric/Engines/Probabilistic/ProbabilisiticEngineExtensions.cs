using Microsoft.Extensions.Logging;

namespace Rubric.Engines.Probabilistic;

internal static class ProbabilisiticEngineExtensions
{

  public const string RANDOM_KEY = "__RANDOM";

  private const string DOES_NOT_APPLY = "Rule {name} does not apply.";
  private const string APPLIES = "Rule {name} does not applies.";
  private const string APPLYING = "Applying {name}.";
  private const string DONE = "Finished applying {name}.";

  /// <summary>
  ///
  ///     Apply an async preprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPreRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    Rules.Probabilistic.Async.IRule<T> r,
    T i,
    CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (e.Random.NextDouble() >= await r.DoesApply(ctx, i, t).ConfigureAwait(false))
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
        return;
      }
      e.Logger.LogTrace("Rule {name} applies.", r.Name);
      e.Logger.LogTrace("Applying {name}.", r.Name);
      await r.Apply(ctx, i, t).ConfigureAwait(false);
      e.Logger.LogTrace("Finished applying {name}.", r.Name);
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, null, t)) throw;
    }
  }

  /// <summary>
  ///     Apply an async postprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPostRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    Rules.Probabilistic.Async.IRule<T> r,
    T o,
    CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (e.Random.NextDouble() >= await r.DoesApply(ctx, o, t).ConfigureAwait(false))
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
        return;
      }
      e.Logger.LogTrace(APPLIES, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      await r.Apply(ctx, o, t).ConfigureAwait(false);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, null, o, t)) throw;
    }
  }

  /// <summary>
  ///     Apply an async rule.  Handle trace logging, exception handling, etc.
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
    Rules.Probabilistic.Async.IRule<TIn, TOut> r,
    TIn i,
    TOut o,
    CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (e.Random.NextDouble() <= await r.DoesApply(ctx, i, o, t).ConfigureAwait(false))
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        e.Logger.LogTrace(APPLIES, r.Name);
        e.Logger.LogTrace(APPLYING, r.Name);
        await r.Apply(ctx, i, o, t).ConfigureAwait(false);
        e.Logger.LogTrace(DONE, r.Name);
      }
      else
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
      }
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, o, t)) throw;
    }
  }

  /// <summary>
  ///     Apply a pre-rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  internal static void ApplyPreRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    Rules.Probabilistic.IRule<T> r,
    T i)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (e.Random.NextDouble() >= r.DoesApply(ctx, i))
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
        return;
      }
      e.Logger.LogTrace(APPLIES, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      r.Apply(ctx, i);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, null)) throw;
    }
  }

  /// <summary>
  ///     Apply a rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">Engine.</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="o">The current o item.</param>
  internal static void ApplyRule<TIn, TOut>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    Rules.Probabilistic.IRule<TIn, TOut> r,
    TIn i,
    TOut o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (e.Random.NextDouble() >= r.DoesApply(ctx, i, o))
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
        return;
      }
      e.Logger.LogTrace(APPLIES, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      r.Apply(ctx, i, o);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, o))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a postprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  internal static void ApplyPostRule<T>(
    this BaseProbabilisticRuleEngine e,
    IEngineContext ctx,
    Rules.Probabilistic.IRule<T> r,
    T o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (e.Random.NextDouble() >= r.DoesApply(ctx, o))
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
        return;
      }
      e.Logger.LogTrace(APPLIES, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      r.Apply(ctx, o);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (ItemHaltException)
    {
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, null, o)) throw;
    }
  }
}
