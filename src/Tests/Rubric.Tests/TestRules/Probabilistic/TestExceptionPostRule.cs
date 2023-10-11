using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestExceptionPostRule : Rule<TestOutput>
{
  public TestExceptionPostRule(bool onDoesApply) => OnDoesApply = onDoesApply;

  public bool OnDoesApply { get; }

  public override void Apply(IEngineContext context, TestOutput obj)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override double DoesApply(IEngineContext context, TestOutput obj)
    => OnDoesApply ? throw new() : 1;
}