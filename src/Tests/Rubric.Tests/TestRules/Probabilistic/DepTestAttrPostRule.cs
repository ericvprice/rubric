using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class DepTestAttrPostRule : Rule<TestOutput>
{
    private readonly double _shouldApply;

    public DepTestAttrPostRule(double shouldApply) => _shouldApply = shouldApply;

    public override void Apply(IEngineContext context, TestOutput obj) => obj.TestFlag = true;

    public override double DoesApply(IEngineContext context, TestOutput obj) => _shouldApply;

}