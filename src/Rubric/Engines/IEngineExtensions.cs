using Microsoft.Extensions.Logging;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric.Engines;

/// <summary>
///   Rule application methods shared among multiple engine implementations.
/// </summary>
internal static class EngineExtensions
{
  /// <summary>
  ///     Apply an async preprocessing rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPreRule<T>(this BaseRuleEngine e, IEngineContext ctx, IAsyncRule<T> r, T i, CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
    using var scope = e.Logger.BeginScope("Rule", r.Name);
    try
    {
      var doesApply = await r.DoesApply(ctx, i, t).ConfigureAwait(false);
      if (doesApply)
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        e.Logger.LogTrace("Rule {name} applies.", r.Name);
        e.Logger.LogTrace("Applying {name}.", r.Name);
        await r.Apply(ctx, i, t).ConfigureAwait(false);
        e.Logger.LogTrace("Finished applying {name}.", r.Name);
      }
      else
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
      }
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, null, t))
      {
        throw;
      }
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
  internal static async Task ApplyAsyncPostRule<T>(this BaseRuleEngine e, IEngineContext ctx, IAsyncRule<T> r, T o, CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
    using var scope = e.Logger.BeginScope("Rule", r.Name);
    try
    {
      var doesApply = await r.DoesApply(ctx, o, t).ConfigureAwait(false);
      if (doesApply)
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        e.Logger.LogTrace("Rule {name} applies.", r.Name);
        e.Logger.LogTrace("Applying {name}.", r.Name);
        await r.Apply(ctx, o, t).ConfigureAwait(false);
        e.Logger.LogTrace("Finished applying {name}.", r.Name);
      }
      else
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
      }
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, null, o, t))
      {
        throw;
      }
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
  internal static async Task ApplyAsyncRule<TIn, TOut>(this BaseRuleEngine e, IEngineContext ctx, IAsyncRule<TIn, TOut> r, TIn i, TOut o, CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
    using var scope = e.Logger.BeginScope("Rule", r.Name);
    try
    {
      var doesApply = await r.DoesApply(ctx, i, o, t).ConfigureAwait(false);
      if (doesApply)
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        e.Logger.LogTrace("Rule {name} applies.", r.Name);
        e.Logger.LogTrace("Applying {name}.", r.Name);
        await r.Apply(ctx, i, o, t).ConfigureAwait(false);
        e.Logger.LogTrace("Finished applying {name}.", r.Name);
      }
      else
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
      }
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, o, t))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a pre-rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  internal static void ApplyPreRule<T>(this BaseRuleEngine e, IEngineContext ctx, IRule<T> r, T i)
  {
    using var scope = e.Logger.BeginScope("Rule", r.Name);
    try
    {
      var doesApply = r.DoesApply(ctx, i);
      if (!doesApply)
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
        return;
      }
      e.Logger.LogTrace("Rule {name} applies.", r.Name);

      e.Logger.LogTrace("Applying {name}.", r.Name);
      r.Apply(ctx, i);
      e.Logger.LogTrace("Finished applying {Name}.", r.Name);
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, i, null))
      {
        throw;
      }
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
  internal static void ApplyRule<TIn, TOut>(this BaseRuleEngine e, IEngineContext ctx, IRule<TIn, TOut> r, TIn i, TOut o)
  {
    using var scope = e.Logger.BeginScope("Rule", r.Name);
    try
    {
      var doesApply = r.DoesApply(ctx, i, o);
      if (!doesApply)
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
        return;
      }
      e.Logger.LogTrace("Rule {name} applies.", r.Name);
      e.Logger.LogTrace("Applying {name}.", r.Name);
      r.Apply(ctx, i, o);
      e.Logger.LogTrace("Finished applying {name}.", r.Name);
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
  internal static void ApplyPostRule<T>(this BaseRuleEngine e, IEngineContext ctx, IRule<T> r, T o)
  {
    using var scope = e.Logger.BeginScope("Rule", r.Name);
    try
    {
      var doesApply = r.DoesApply(ctx, o);
      if (!doesApply)
      {
        e.Logger.LogTrace("Rule {name} does not apply.", r.Name);
        return;
      }
      e.Logger.LogTrace("Rule {name} does not applies.", r.Name);
      e.Logger.LogTrace("Applying {name}.", r.Name);
      r.Apply(ctx, o);
      e.Logger.LogTrace("Finished applying {name}.", r.Name);
    }
    catch (ItemHaltException)
    {
    }
    catch (Exception ex)
    {
      if (!e.HandleException(ex, e, ctx, r, null, o))
      {
        throw;
      }
    }
  }
}
