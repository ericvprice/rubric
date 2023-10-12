using Rubric.Rules.Async;

namespace Rubric.Tests.TestRules.Async;

public class TestPreRule : Rule<TestInput>
{
  private readonly bool _flagValue;
  private readonly bool _shouldApply;

  public TestPreRule(bool shouldApply, bool flagValue = true)
  {
    _shouldApply = shouldApply;
    _flagValue = flagValue;
  }

  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
  {
    obj.InputFlag = _flagValue;
    return Task.CompletedTask;
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput obj, CancellationToken token)
    => Task.FromResult(_shouldApply);
}