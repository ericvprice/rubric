using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestDefaultPreRule : DefaultRule<TestInput>
{
  public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = true;
}