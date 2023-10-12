using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestExceptionPostRule : Rule<TestOutput>
{
  private readonly bool _onDoesApply;

  public TestExceptionPostRule(bool onDoesApply) => _onDoesApply = onDoesApply;

  public override void Apply(IEngineContext context, TestOutput obj)
  {
    obj.TestFlag = true;
    throw new();
  }

  public override double DoesApply(IEngineContext context, TestOutput obj)
    => _onDoesApply ? throw new() : 1;
}