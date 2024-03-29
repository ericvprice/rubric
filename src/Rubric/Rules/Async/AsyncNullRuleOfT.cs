﻿namespace Rubric.Rules.Async;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="T">The input type.</typeparam>
public class AsyncNullRule<T> : AsyncRule<T> where T : class
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, T input, CancellationToken t) 
    => Task.CompletedTask;

  /// <inheritdoc />
  public override Task<bool> DoesApply(IEngineContext context, T input, CancellationToken t)
    => Task.FromResult(false);

}