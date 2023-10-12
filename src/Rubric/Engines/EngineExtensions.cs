using Microsoft.Extensions.Logging;
using Rubric.Engines.Default;

namespace Rubric.Engines;

/// <summary>
///   Rule application methods shared among multiple engine implementations.
/// </summary>
internal static class EngineExtensions
{

  private const string DOES_NOT_APPLY = "Rule {name} does not apply.";
  private const string APPLIES = "Rule {name} does not applies.";
  private const string APPLYING = "Applying {name}.";
  private const string DONE = "Finished applying {name}.";

  /// <summary>
  ///     Apply an async preprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPreRule<T>(this BaseRuleEngine e, IEngineContext ctx, Rules.Async.IRule<T> r, T i, CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (!await r.DoesApply(ctx, i, t).ConfigureAwait(false))
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
        return;
      }
      using var logCtx = e.Logger.BeginScope(r.Name);
      e.Logger.LogTrace(APPLIES, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      await r.Apply(ctx, i, t).ConfigureAwait(false);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, null, t)) throw;
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
  internal static async Task ApplyAsyncPostRule<T>(this BaseRuleEngine e, IEngineContext ctx, Rules.Async.IRule<T> r, T o, CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (!await r.DoesApply(ctx, o, t).ConfigureAwait(false))
      {
        e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
        return;
      }
      using var logCtx = e.Logger.BeginScope(r.Name);
        e.Logger.LogTrace(APPLIES, r.Name);
        e.Logger.LogTrace(APPLYING, r.Name);
        await r.Apply(ctx, o, t).ConfigureAwait(false);
        e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, null, o, t)) throw;
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
  internal static async Task ApplyAsyncRule<TIn, TOut>(this BaseRuleEngine e, IEngineContext ctx, Rules.Async.IRule<TIn, TOut> r, TIn i, TOut o, CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (!await r.DoesApply(ctx, i, o, t).ConfigureAwait(false))
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
        return;
      }
      using var logCtx = e.Logger.BeginScope(r.Name);
      e.Logger.LogTrace(APPLIES, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      await r.Apply(ctx, i, o, t).ConfigureAwait(false);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, o, t)) throw;
    }
  }

  /// <summary>
  ///     Apply a pre-rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  internal static void ApplyPreRule<T>(this BaseRuleEngine e, IEngineContext ctx, Rules.IRule<T> r, T i)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (!r.DoesApply(ctx, i))
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
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, null)) throw;
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
  internal static void ApplyRule<TIn, TOut>(this BaseRuleEngine e, IEngineContext ctx, Rules.IRule<TIn, TOut> r, TIn i, TOut o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (!r.DoesApply(ctx, i, o))
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
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, i, o)) throw;
    }
  }

  /// <summary>
  ///     Apply a postprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  internal static void ApplyPostRule<T>(this BaseRuleEngine e, IEngineContext ctx, Rules.IRule<T> r, T o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (!r.DoesApply(ctx, o))
      {
        e.Logger.LogTrace(APPLIES, r.Name);
        return;
      }
      e.Logger.LogTrace(DOES_NOT_APPLY, r.Name);
      e.Logger.LogTrace(APPLYING, r.Name);
      r.Apply(ctx, o);
      e.Logger.LogTrace(DONE, r.Name);
    }
    catch (Exception ex)
    {
      if (!BaseRuleEngine.HandleException(ex, e, ctx, r, null, o)) throw;
    }
  }
}
