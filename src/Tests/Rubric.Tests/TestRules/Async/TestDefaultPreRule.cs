using Rubric.Rules.Async;

namespace Rubric.Tests.TestRules.Async;

public class TestDefaultPreRule : DefaultAsyncRule<TestInput>
{
  public override string Name => nameof(TestRules.TestDefaultPostRule);

  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
  {
    obj.InputFlag = true;
    return Task.CompletedTask;
  }
}