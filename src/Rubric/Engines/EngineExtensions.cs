using Microsoft.Extensions.Logging;
using Rubric.Engines.Implementation;
using Rubric.Rules.Async;

namespace Rubric.Engines;

/// <summary>
///   Rule application methods shared among multiple engine implementations.
/// </summary>
internal static class EngineExtensions
{
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
    this BaseRuleEngine e,
    IEngineContext ctx,
    IRule<T> r,
    T i,
    CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (await r.DoesApply(ctx, i, t).ConfigureAwait(false))
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        _appliesLogger(e.Logger, r.Name, null);
        _applyingLogger(e.Logger, r.Name, null);
        await r.Apply(ctx, i, t).ConfigureAwait(false);
        _doneLogger(e.Logger, r.Name, null);
      }
      else
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
      }
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
    this BaseRuleEngine e,
    IEngineContext ctx,
    IRule<T> r,
    T o,
    CancellationToken t)
  {
    try
    {
      t.ThrowIfCancellationRequested();
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (await r.DoesApply(ctx, o, t).ConfigureAwait(false))
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        _appliesLogger(e.Logger, r.Name, null);
        _applyingLogger(e.Logger, r.Name, null);
        await r.Apply(ctx, o, t).ConfigureAwait(false);
        _doneLogger(e.Logger, r.Name, null);
      }
      else
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
      }
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
    this BaseRuleEngine e,
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
      if (await r.DoesApply(ctx, i, o, t).ConfigureAwait(false))
      {
        using var logCtx = e.Logger.BeginScope(r.Name);
        _appliesLogger(e.Logger, r.Name, null);
        _applyingLogger(e.Logger, r.Name, null);
        await r.Apply(ctx, i, o, t).ConfigureAwait(false);
        _doneLogger(e.Logger, r.Name, null);
      }
      else
      {
        _doesNotApplyLogger(e.Logger, r.Name, null);
      }
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
    this BaseRuleEngine e,
    IEngineContext ctx,
    Rules.IRule<T> r,
    T i)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (r.DoesApply(ctx, i))
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
    this BaseRuleEngine e,
    IEngineContext ctx,
    Rules.IRule<TIn, TOut> r,
    TIn i,
    TOut o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (r.DoesApply(ctx, i, o))
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
    this BaseRuleEngine e,
    IEngineContext ctx,
    Rules.IRule<T> r,
    T o)
  {
    try
    {
      using var scope = e.Logger.BeginScope("Rule", r.Name);
      if (r.DoesApply(ctx, o))
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