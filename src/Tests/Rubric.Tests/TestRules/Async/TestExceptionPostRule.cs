namespace Rubric.Tests.TestRules.Async;

public class TestExceptionPostRule : Rubric.Rules.Async.Rule<TestOutput>
{
  public TestExceptionPostRule(bool onDoesApply)
    => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override Task<bool> DoesApply(IEngineContext context, TestOutput obj, CancellationToken token)
    => OnDoesApply ? throw new() : Task.FromResult(true);
}