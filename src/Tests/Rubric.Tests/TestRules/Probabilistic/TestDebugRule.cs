using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestDebugRule<T> : DebugRule<T>
{
  /// <inheritdoc />
  public override void Apply(IEngineContext context, T input) { }
}

public class TestDebugRule<TIn, TOut> : DebugRule<TIn, TOut>
{
  /// <inheritdoc />
  public override void Apply(IEngineContext context, TIn input, TOut output) { }
}
