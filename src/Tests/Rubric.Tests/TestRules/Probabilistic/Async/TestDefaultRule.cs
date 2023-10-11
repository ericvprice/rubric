using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestDefaultRule : DefaultRule<TestInput, TestOutput>
{
  public override Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken t)
  {
    input.InputFlag = output.TestFlag = true;
    return Task.CompletedTask;
  }
}