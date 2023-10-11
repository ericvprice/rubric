using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestExceptionPreRule : Rule<TestInput>
{
  public TestExceptionPreRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken t)
  {
    obj.InputFlag = true;
    throw new();
  }

  public override Task<double> DoesApply(IEngineContext context, TestInput obj, CancellationToken t) =>
    OnDoesApply ? throw new() : Task.FromResult(1D);
}