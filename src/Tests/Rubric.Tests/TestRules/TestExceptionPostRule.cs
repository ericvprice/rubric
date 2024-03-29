namespace Rubric.Tests.TestRules;

public class TestExceptionPostRule : Rule<TestOutput>
{
  public TestExceptionPostRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override void Apply(IEngineContext context, TestOutput obj)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override bool DoesApply(IEngineContext context, TestOutput obj)
    => OnDoesApply ? throw new() : true;
}