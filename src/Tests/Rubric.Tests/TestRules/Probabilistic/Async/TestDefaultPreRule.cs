using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestDefaultPreRule : DefaultRule<TestInput>
{
  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken t)
  {
    obj.InputFlag = true;
    return Task.CompletedTask;
  }
}