using Rubric.Rules;

namespace Rubric.Tests.TestRules;

public class TestExceptionPreRule : Rule<TestInput>
{
  public TestExceptionPreRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override void Apply(IEngineContext context, TestInput obj)
  {
    obj.InputFlag = true;
    throw new();
  }

  public override bool DoesApply(IEngineContext context, TestInput obj) =>
    OnDoesApply ? throw new() : true;
}