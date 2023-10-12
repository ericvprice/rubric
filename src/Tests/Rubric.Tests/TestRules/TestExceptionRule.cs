using Rubric.Rules;

namespace Rubric.Tests.TestRules;

public class TestExceptionRule : Rule<TestInput, TestOutput>
{
  public TestExceptionRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  private readonly bool _onDoesApply;

  public override void Apply(IEngineContext context, TestInput obj, TestOutput output)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override bool DoesApply(IEngineContext context, TestInput obj, TestOutput output)
    => _onDoesApply ? throw new() : true;
}