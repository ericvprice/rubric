using Rubric.Dependency;
namespace Rubric.Tests.TestRules.Probabilistic;

[DependsOn(typeof(DepTestAttrPostRule))]
public class DepTestAttrPostRule2 : DepTestAttrPostRule {

    public DepTestAttrPostRule2(double shouldApply) : base(shouldApply) { }

}