namespace RulesEngine.Tests.TestRules.Async
{
  public class TestExceptionAsyncPostRule : AsyncRule<TestOutput>
  {
    public TestExceptionAsyncPostRule(bool onDoesApply)
        => OnDoesApply = onDoesApply;

    public bool OnDoesApply { get; }

    public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
    {
      obj.TestFlag = true;
      throw new Exception();
    }

    public override Task<bool> DoesApply(IEngineContext context, TestOutput obj, CancellationToken token)
        => OnDoesApply ? throw new Exception() : Task.FromResult(true);
  }
}