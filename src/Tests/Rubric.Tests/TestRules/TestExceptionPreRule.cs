using Rubric.Rules;

namespace Rubric.Tests.TestRules;

public class TestExceptionPreRule : Rule<TestInput>
{
  private readonly bool _onDoesApply;

  public TestExceptionPreRule(bool onDoesApply) => _onDoesApply = onDoesApply;
  
  public override void Apply(IEngineContext context, TestInput obj)
  {
    obj.InputFlag = true;
    throw new();
  }

  public override bool DoesApply(IEngineContext context, TestInput obj) =>
    _onDoesApply ? throw new() : true;
}