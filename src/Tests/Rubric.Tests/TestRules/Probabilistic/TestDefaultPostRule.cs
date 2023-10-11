using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestDefaultPostRule : DefaultRule<TestOutput>
{
  public override string Name => nameof(TestDefaultPostRule);

  public override void Apply(IEngineContext context, TestOutput obj)
    => obj.TestFlag = true;
}