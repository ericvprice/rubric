using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestPostRule : Rule<TestOutput>
{
  private readonly double _shouldApply;

  public TestPostRule(double shouldApply) => _shouldApply = shouldApply;

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken t)
  {
    obj.TestFlag = true;
    return Task.CompletedTask;
  }

  public override Task<double> DoesApply(IEngineContext context, TestOutput obj, CancellationToken t) => Task.FromResult(_shouldApply);
}