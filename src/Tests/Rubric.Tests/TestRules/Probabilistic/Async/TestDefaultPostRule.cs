using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestDefaultPostRule : DefaultRule<TestOutput>
{
  public override string Name => nameof(TestDefaultPostRule);

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken t)
  {
    obj.TestFlag = true;
    return Task.CompletedTask;
  }
}