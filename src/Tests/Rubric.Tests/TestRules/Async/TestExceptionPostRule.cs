namespace Rubric.Tests.TestRules.Async;

public class TestExceptionPostRule : Rubric.Rules.Async.Rule<TestOutput>
{
  private readonly bool _onDoesApply;

  public TestExceptionPostRule(bool onDoesApply)
    => _onDoesApply = onDoesApply;

  
  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override Task<bool> DoesApply(IEngineContext context, TestOutput obj, CancellationToken token)
    => _onDoesApply ? throw new() : Task.FromResult(true);
}