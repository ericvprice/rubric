using Microsoft.Extensions.Logging;
using Rubric.Rules;
using Rubric.Rules.Async;

namespace Rubric;

internal static class EngineExtensions
{
  /// <summary>
  ///     Apply an async r.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPreRule<T>(this IRuleEngine e, IEngineContext ctx, IAsyncRule<T> r, T i, CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
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
      if (!HandleException(ex, e, ctx, r, i, null, t))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply an async r.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncPostRule<T>(this IRuleEngine e, IEngineContext ctx, IAsyncRule<T> r, T o, CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
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
      if (!HandleException(ex, e, ctx, r, null, o, t))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply an async r.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="o">The o item.</param>
  /// <param name="t">The cancellation t.</param>
  internal static async Task ApplyAsyncRule<TIn, TOut>(this IRuleEngine e, IEngineContext ctx, IAsyncRule<TIn, TOut> r, TIn i, TOut o, CancellationToken t)
  {
    t.ThrowIfCancellationRequested();
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
      if (!HandleException(ex, e, ctx, r, i, o, t))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a pre-r.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  internal static void ApplyPreRule<T>(this IRuleEngine e, IEngineContext ctx, IRule<T> r, T i)
  {
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
      if (!HandleException(ex, e, ctx, r, i, null))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a r.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">Engine.</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="i">The current i item.</param>
  /// <param name="o">The current o item.</param>
  internal static void ApplyRule<TIn, TOut>(this IRuleEngine e, IEngineContext ctx, IRule<TIn, TOut> r, TIn i, TOut o)
  {
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
      if (!HandleException(ex, e, ctx, r, i, o))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a post r.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="e">The e</param>
  /// <param name="ctx">Engine context.</param>
  /// <param name="r">The current r.</param>
  /// <param name="o">The o item.</param>
  internal static void ApplyPostRule<T>(this IRuleEngine e, IEngineContext ctx, IRule<T> r, T o)
  {
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
    catch (Exception ex)
    {
      if (!HandleException(ex, e, ctx, r, null, o))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///   Consistently handle exceptions after attempting to run a r.
  /// </summary>
  /// <param name="ex">The exception.</param>
  /// <param name="e">The e.</param>
  /// <param name="ctx">The current context.</param>
  /// <param name="rule">The r being executed.</param>
  /// <param name="input">The current i object.</param>
  /// <param name="output">The current o object.</param>
  /// <param name="t">The cancellation t, if present.</param>
  /// <returns>Whether the exception should be rethrown.</returns>
  private static bool HandleException(Exception ex, IRuleEngine e, IEngineContext ctx, object rule, object input, object output, CancellationToken t = default)
  {
    switch (ex)
    {
      case TaskCanceledException tce:
        if (t == tce.CancellationToken)
        {
          return false;
        }
        try
        {
          return e.ExceptionHandler.HandleException(ex, ctx, input, null, rule);
        }
        catch (EngineException ee)
        {
          ee.Rule = rule;
          ee.Input = input;
          ee.Output = output;
          ee.Context = ctx;
          e.LastException = ee;
          throw;
        }
      case EngineException ee:
        ee.Rule = rule;
        ee.Input = input;
        ee.Output = output;
        ee.Context = ctx;
        e.LastException = ee;
        return false;
      default:
        try
        {
          return e.ExceptionHandler.HandleException(ex, ctx, input, null, rule);
        }
        catch (EngineException ee)
        {
          ee.Rule = rule;
          ee.Input = input;
          ee.Output = output;
          ee.Context = ctx;
          e.LastException = ee;
          throw;
        }
    }
  }

}
