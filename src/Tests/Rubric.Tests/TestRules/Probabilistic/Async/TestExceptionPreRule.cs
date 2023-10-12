using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestExceptionPreRule : Rule<TestInput>
{
  public TestExceptionPreRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  private readonly bool _onDoesApply;

  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken t)
  {
    obj.InputFlag = true;
    throw new();
  }

  public override Task<double> DoesApply(IEngineContext context, TestInput obj, CancellationToken t) =>
    _onDoesApply ? throw new() : Task.FromResult(1D);
}