namespace Rubric.Tests.TestRules.Async
{
  public class TestDefaultAsyncPreRule : DefaultAsyncRule<TestInput>
  {
    public override string Name => nameof(TestDefaultPostRule);

    public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
    {
      obj.InputFlag = true;
      return Task.CompletedTask;
    }
  }
}