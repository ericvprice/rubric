using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestExceptionRule : Rule<TestInput, TestOutput>
{
  private readonly bool _onDoesApply;

  public TestExceptionRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  public override void Apply(IEngineContext context, TestInput obj, TestOutput output)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override double DoesApply(IEngineContext context, TestInput obj, TestOutput output)
    => _onDoesApply ? throw new() : 1D;
}