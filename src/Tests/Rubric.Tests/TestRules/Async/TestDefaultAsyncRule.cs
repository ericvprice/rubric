namespace Rubric.Tests.TestRules.Async;

public class TestDefaultAsyncRule : DefaultAsyncRule<TestInput, TestOutput>
{
  public override Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
  {
    input.InputFlag = true;
    output.TestFlag = true;
    return Task.CompletedTask;
  }
}