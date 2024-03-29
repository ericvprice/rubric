﻿namespace Rubric.Rules.Async;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public class AsyncNullRule<TIn, TOut> : AsyncRule<TIn, TOut>
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken t) 
    => Task.CompletedTask;

  /// <inheritdoc />
  public override Task<bool> DoesApply(IEngineContext context, TIn input, TOut output, CancellationToken t)
    => Task.FromResult(false);
}