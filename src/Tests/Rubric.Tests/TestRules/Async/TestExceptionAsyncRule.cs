namespace Rubric.Tests.TestRules.Async;

public class TestExceptionAsyncRule : Rubric.Rules.Async.Rule<TestInput, TestOutput>
{
  public TestExceptionAsyncRule(bool onDoesApply)
    => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override Task Apply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken token)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken token)
    => OnDoesApply ? throw new() : Task.FromResult(true);
}