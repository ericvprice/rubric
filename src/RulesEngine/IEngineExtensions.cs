using Microsoft.Extensions.Logging;
using RulesEngine.Rules;
using RulesEngine.Rules.Async;

namespace RulesEngine;

#pragma warning disable CA2254 // Template should be a static expression

internal static class IEngineExtensions
{
  /// <summary>
  ///     Apply an asyc rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="ctx">Engine context.</param>
  /// <param name="rule">The current rule.</param>
  /// <param name="input">The current input item.</param>
  internal static async Task ApplyAsyncPreRule<T>(this IRulesEngine engine, IEngineContext ctx, IAsyncRule<T> r, T i, CancellationToken t)
  {
    try
    {
      var doesApply = await r.DoesApply(ctx, i, t).ConfigureAwait(false);
      if (doesApply)
      {
        using var logCtx = engine.Logger.BeginScope(r.Name);
        engine.Logger.LogTrace($"Rule {r.Name} applies.");
        engine.Logger.LogTrace($"Applying {r.Name}.");
        await r.Apply(ctx, i, t).ConfigureAwait(false);
        engine.Logger.LogTrace($"Finished applying {r.Name}.");
      }
      else
      {
        engine.Logger.LogTrace($"Rule {r.Name} does not apply.");
      }
    }
    catch (Exception e)
    {
      if (!HandleException(e, engine, ctx, r, i, null))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a prerule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="ctx">Engine context.</param>
  /// <param name="rule">The current rule.</param>
  /// <param name="input">The current input item.</param>
  internal static void ApplyPreRule<T>(this IRulesEngine engine, IEngineContext ctx, IRule<T> rule, T input)
  {
    try
    {
      var doesApply = rule.DoesApply(ctx, input);
      engine.Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
      if (!doesApply) return;
      engine.Logger.LogTrace($"Applying {rule.Name}.");
      rule.Apply(ctx, input);
      engine.Logger.LogTrace($"Finished applying {rule.Name}.");
    }
    catch (Exception e)
    {
      if (!HandleException(e, engine, ctx, rule, input, null))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="ctx">Engine context.</param>
  /// <param name="rule">The current rule.</param>
  /// <param name="input">The current input item.</param>
  /// <param name="output">The current output item.</param>
  internal static void ApplyRule<TIn, TOut>(this IRulesEngine engine, IEngineContext ctx, IRule<TIn, TOut> rule, TIn input, TOut output)
  {
    try
    {
      var doesApply = rule.DoesApply(ctx, input, output);
      engine.Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
      if (!doesApply) return;
      engine.Logger.LogTrace($"Applying {rule.Name}.");
      rule.Apply(ctx, input, output);
      engine.Logger.LogTrace($"Finished applying {rule.Name}.");
    }
    catch (Exception e)
    {
      if (!HandleException(e, engine, ctx, rule, input, output))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///     Apply a post rule.  Handle trace logging, exception handling, etc.
  /// </summary>
  /// <param name="ctx">Engine context.</param>
  /// <param name="rule">The current rule.</param>
  /// <param name="output">The current output item.</param>
  internal static void ApplyPostRule<T>(this IRulesEngine engine, IEngineContext ctx, IRule<T> rule, T output)
  {
    try
    {
      var doesApply = rule.DoesApply(ctx, output);
      engine.Logger.LogTrace($"Rule {rule.Name} {(doesApply ? "does" : "does not")} apply.");
      if (!doesApply) return;
      engine.Logger.LogTrace($"Applying {rule.Name}.");
      rule.Apply(ctx, output);
      engine.Logger.LogTrace($"Finished applying {rule.Name}.");
    }
    catch (Exception e)
    {
      if(!HandleException(e, engine, ctx, rule, null, output))
      {
        throw;
      }
    }
  }

  /// <summary>
  ///   Consistently handle exceptions after attempting to run a rule.
  /// </summary>
  /// <param name="e">The exception.</param>
  /// <param name="engine">The engine.</param>
  /// <param name="ctx">The current context.</param>
  /// <param name="rule">The rule being executed.</param>
  /// <param name="input">The current input object.</param>
  /// <param name="output">The current output object.</param>
  /// <returns>Whether the exception should be rethrown.</returns>
  private static bool HandleException(Exception e, IRulesEngine engine, IEngineContext ctx, object rule, object input, object output)
  {
    switch (e)
    {
      case EngineException ee:
        ee.Rule = rule;
        ee.Input = input;
        ee.Output = output;
        ee.Context = ctx;
        engine.LastException = ee;
        return false;
      default:
        try
        {
          return engine.ExceptionHandler.HandleException(e, ctx, input, null, rule);
        }
        catch (EngineException ee)
        {
          ee.Rule = rule;
          ee.Input = input;
          ee.Output = output;
          ee.Context = ctx;
          engine.LastException = ee;
          throw;
        }
    }
  }

}

