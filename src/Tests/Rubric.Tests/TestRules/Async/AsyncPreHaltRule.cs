namespace Rubric.Tests.TestRules.Async;

public class AsyncPreHaltRule : DefaultAsyncRule<TestInput>
{
  public override Task Apply(IEngineContext context, TestInput input, CancellationToken token)
    => throw new EngineHaltException();

}