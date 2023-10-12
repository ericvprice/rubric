namespace Rubric.Tests.TestRules.Async;

public class TestExceptionRule : Rubric.Rules.Async.Rule<TestInput, TestOutput>
{
  private readonly bool _onDoesApply;

  public TestExceptionRule(bool onDoesApply)
    => _onDoesApply = onDoesApply;

  public override Task Apply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken token)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput obj, TestOutput output, CancellationToken token)
    => _onDoesApply ? throw new() : Task.FromResult(true);
}