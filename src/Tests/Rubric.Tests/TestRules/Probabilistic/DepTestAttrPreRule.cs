using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class DepTestAttrPreRule : Rule<TestInput>
{
    private readonly bool _flagValue;
    private readonly double _shouldApply;

    public DepTestAttrPreRule(double shouldApply, bool flagValue = true)
    {
        _flagValue = flagValue;
        _shouldApply = shouldApply;
    }

    public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = _flagValue;

    public override double DoesApply(IEngineContext context, TestInput obj) => _shouldApply;
}