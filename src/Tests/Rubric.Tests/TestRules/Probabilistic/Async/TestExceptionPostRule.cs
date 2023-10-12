using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestExceptionPostRule : Rule<TestOutput>
{
  private readonly bool _onDoesApply;

  public TestExceptionPostRule(bool onDoesApply) => _onDoesApply = onDoesApply;
  
  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken t)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override Task<double> DoesApply(IEngineContext context, TestOutput obj, CancellationToken t)
    => _onDoesApply ? throw new() : Task.FromResult(1D);
}