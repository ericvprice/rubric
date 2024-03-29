namespace Rubric.Tests.TestRules.Async;

public class TestDefaultAsyncPostRule : DefaultAsyncRule<TestOutput>
{
  public override string Name => nameof(TestDefaultPostRule);

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
  {
    obj.TestFlag = true;
    return Task.CompletedTask;
  }
}