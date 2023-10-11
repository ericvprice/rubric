using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestDebugRule<T> : DebugRule<T> where T: class
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, T input, CancellationToken t) => Task.CompletedTask;
}

public class TestDebugRule<TIn, TOut> : DebugRule<TIn, TOut>
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken t) => Task.CompletedTask;
}
