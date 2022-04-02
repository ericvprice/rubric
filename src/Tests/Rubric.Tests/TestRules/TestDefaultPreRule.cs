namespace Rubric.Tests.TestRules;

public class TestDefaultPreRule : DefaultRule<TestInput>
{
  public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = true;
}