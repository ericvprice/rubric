using Rubric.Rules;

namespace Rubric.Tests.TestRules;

public class TestDefaultPostRule : DefaultRule<TestOutput>
{
  public override string Name => nameof(TestDefaultPostRule);

  public override void Apply(IEngineContext context, TestOutput obj)
    => obj.TestFlag = true;
}