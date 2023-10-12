using Rubric.Rules.Async;

namespace Rubric.Tests.TestRules.Async;

public class TestDefaultPostRule : DefaultAsyncRule<TestOutput>
{
  public override string Name => nameof(TestRules.TestDefaultPostRule);

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
  {
    obj.TestFlag = true;
    return Task.CompletedTask;
  }
}