namespace RulesEngine.Tests.TestRules.Async
{
  public class TestAsyncPostRule : AsyncRule<TestOutput>
  {
    private readonly bool _shouldApply;

    public TestAsyncPostRule(bool shouldApply) => _shouldApply = shouldApply;

    public override Task Apply(IEngineContext context, TestOutput obj)
    {
      obj.TestFlag = true;
      return Task.CompletedTask;
    }

    public override Task<bool> DoesApply(IEngineContext context, TestOutput obj)
        => Task.FromResult(_shouldApply);
  }
}