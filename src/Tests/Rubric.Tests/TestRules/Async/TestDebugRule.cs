namespace Rubric.Tests.TestRules.Async;

public class TestAsyncDebugRule<T> : AsyncDebugRule<T> where T : class
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, T input, CancellationToken t) => Task.CompletedTask;
}

public class TestAsyncDebugRule<TIn, TOut> : AsyncDebugRule<TIn, TOut>
{
  /// <inheritdoc />
  public override Task Apply(IEngineContext context, TIn input, TOut output, CancellationToken t) => Task.CompletedTask;
}
