using Rubric.Rules;

namespace Rubric.Tests.TestRules;

public class TestExceptionPostRule : Rule<TestOutput>
{
  public TestExceptionPostRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  private readonly bool _onDoesApply;
  
  public override void Apply(IEngineContext context, TestOutput obj)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override bool DoesApply(IEngineContext context, TestOutput obj)
    => _onDoesApply ? throw new() : true;
}