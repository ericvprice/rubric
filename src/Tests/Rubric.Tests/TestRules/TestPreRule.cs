namespace Rubric.Tests.TestRules;

public class TestPreRule : Rule<TestInput>
{
  private readonly bool _flagValue;
  private readonly bool _shouldApply;

  public TestPreRule(bool shouldApply, bool flagValue = true)
  {
    _flagValue = flagValue;
    _shouldApply = shouldApply;
  }

  public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = _flagValue;

  public override bool DoesApply(IEngineContext context, TestInput obj) => _shouldApply;
}