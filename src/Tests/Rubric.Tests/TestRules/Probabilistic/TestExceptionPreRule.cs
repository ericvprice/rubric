using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestExceptionPreRule : Rule<TestInput>
{
  private readonly bool _onDoesApply;

  public TestExceptionPreRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  public override void Apply(IEngineContext context, TestInput obj)
  {
    obj.InputFlag = true;
    throw new();
  }

  public override double DoesApply(IEngineContext context, TestInput obj) =>
    _onDoesApply ? throw new() : 1;
}