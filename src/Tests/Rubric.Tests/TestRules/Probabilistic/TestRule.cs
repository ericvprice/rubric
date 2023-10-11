using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

internal class TestRule : Rule<TestInput, TestOutput>
{
  private readonly double _expected;

  private readonly bool _flagValue;

  public TestRule(double expected, bool flagValue = true)
  {
    _expected = expected;
    _flagValue = flagValue;
  }

  public override void Apply(IEngineContext context, TestInput input, TestOutput output)
    => input.InputFlag = output.TestFlag = _flagValue;

  public override double DoesApply(IEngineContext context, TestInput input, TestOutput output)
    => _expected;
}