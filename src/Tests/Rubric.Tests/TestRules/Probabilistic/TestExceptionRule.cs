using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestExceptionRule : Rule<TestInput, TestOutput>
{
  public TestExceptionRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override void Apply(IEngineContext context, TestInput obj, TestOutput output)
  {
    obj.InputFlag = output.TestFlag = true;
    throw new();
  }

  public override double DoesApply(IEngineContext context, TestInput obj, TestOutput output)
    => OnDoesApply ? throw new() : 1D;
}