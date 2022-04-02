namespace Rubric.Tests.TestRules;

public class TestExceptionRule : Rule<TestInput, TestOutput>
{
  public TestExceptionRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override void Apply(IEngineContext context, TestInput obj, TestOutput output)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override bool DoesApply(IEngineContext context, TestInput obj, TestOutput output)
    => OnDoesApply ? throw new() : true;
}