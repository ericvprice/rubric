namespace Rubric.Tests.TestRules.Async;

public class TestExceptionPreRule : Rubric.Rules.Async.Rule<TestInput>
{
  private readonly bool _onDoesApply;

  public TestExceptionPreRule(bool onDoesApply)
    => _onDoesApply = onDoesApply;

  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
  {
    obj.InputFlag = true;
    throw new();
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput obj, CancellationToken token)
    => _onDoesApply ? throw new() : Task.FromResult(true);
}