using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestExceptionRule : Rule<TestInput, TestOutput>
{
  private readonly bool _onDoesApply
    ;
  public TestExceptionRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  public override Task Apply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken t)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override Task<double> DoesApply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken t)
    => _onDoesApply ? throw new() : Task.FromResult(1D);
}