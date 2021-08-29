namespace RulesEngine.Tests.TestRules.Async
{
  public class TestExceptionAsyncPreRule : AsyncRule<TestInput>
  {
    public TestExceptionAsyncPreRule(bool onDoesApply)
        => OnDoesApply = onDoesApply;

    public bool OnDoesApply { get; }

    public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
    {
      obj.InputFlag = true;
      throw new Exception();
    }

    public override Task<bool> DoesApply(IEngineContext context, TestInput obj, CancellationToken token)
        => OnDoesApply ? throw new Exception() : Task.FromResult(true);
  }
}