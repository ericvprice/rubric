namespace RulesEngine.Tests.TestRules.Async
{
  public class TestExceptionAsyncPostRule : AsyncRule<TestOutput>
  {
    public TestExceptionAsyncPostRule(bool onDoesApply)
        => OnDoesApply = onDoesApply;

    public bool OnDoesApply { get; }

    public override Task Apply(IEngineContext context, TestOutput obj)
    {
      obj.TestFlag = true;
      throw new Exception();
    }

    public override Task<bool> DoesApply(IEngineContext context, TestOutput obj)
        => OnDoesApply ? throw new Exception() : Task.FromResult(true);
  }
}