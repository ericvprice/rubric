namespace RulesEngine.Tests.TestRules.Async
{
  public class TestExceptionAsyncRule : AsyncRule<TestInput, TestOutput>
  {
    public TestExceptionAsyncRule(bool onDoesApply)
        => OnDoesApply = onDoesApply;

    public bool OnDoesApply { get; }

    public override Task Apply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken token)
    {
      obj.InputFlag = output.TestFlag = true;
      throw new Exception();
    }

    public override Task<bool> DoesApply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken token)
        => OnDoesApply ? throw new Exception() : Task.FromResult(true);
  }
}