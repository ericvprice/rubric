﻿using System.Diagnostics.CodeAnalysis;

namespace Rubric.Rules;

/// <summary>
///   Utility rule that never executes and does nothing, but can serve as a provider of dependencies for other rules
///   to organize execution.
/// </summary>
/// <typeparam name="TIn">The input type.</typeparam>
/// <typeparam name="TOut">The output type.</typeparam>
public class NullRule<TIn, TOut> : Rule<TIn, TOut>
{
  /// <inheritdoc />
  [ExcludeFromCodeCoverage]
  public override void Apply(IEngineContext context, TIn input, TOut output) { }

  /// <inheritdoc />
  public override bool DoesApply(IEngineContext context, TIn input, TOut output)
    => false;
}