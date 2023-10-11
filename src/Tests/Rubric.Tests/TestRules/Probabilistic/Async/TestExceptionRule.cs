using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestExceptionRule : Rule<TestInput, TestOutput>
{
  public TestExceptionRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override Task Apply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken t)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override Task<double> DoesApply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken t)
    => OnDoesApply ? throw new() : Task.FromResult(1D);
}