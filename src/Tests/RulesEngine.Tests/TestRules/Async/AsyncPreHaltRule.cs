namespace RulesEngine.Tests.TestRules.Async
{
  public class AsyncPreHaltRule : DefaultAsyncRule<TestInput>
  {
    public override Task Apply(IEngineContext context, TestInput input)
        => throw new EngineHaltException();

  }
}