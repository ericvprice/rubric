using Rubric.Dependency;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

[DependsOn(typeof(DepTestAttrPostRule))]
public class DepTestAttrPostRule2 : DepTestAttrPostRule
{

    public DepTestAttrPostRule2(double shouldApply) : base(shouldApply) { }
}