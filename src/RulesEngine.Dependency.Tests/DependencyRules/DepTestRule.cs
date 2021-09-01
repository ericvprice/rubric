using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine.Tests.DependencyRules
{
  [DependsOn("dep1")]
  [DependsOn("dep2")]
  [Provides("dep3")]
  internal class DepTestRule : Rule<TestInput, TestOutput>
  {
    private readonly bool _expected;

    private readonly bool _flagValue;

    public DepTestRule(bool expected, bool flagValue = true)
    {
      _expected = expected;
      _flagValue = flagValue;
    }

    public override void Apply(IEngineContext context, TestInput input, TestOutput output)
        => input.InputFlag = output.TestFlag = _flagValue;

    public override bool DoesApply(IEngineContext context, TestInput input, TestOutput output)
        => _expected;
  }
}