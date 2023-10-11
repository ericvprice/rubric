using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestExceptionPostRule : Rule<TestOutput>
{
  public TestExceptionPostRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken t)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override Task<double> DoesApply(IEngineContext context, TestOutput obj, CancellationToken t)
    => OnDoesApply ? throw new() : Task.FromResult(1D);
}