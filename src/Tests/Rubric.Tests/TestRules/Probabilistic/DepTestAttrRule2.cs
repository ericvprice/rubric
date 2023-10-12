using Rubric.Dependency;

namespace Rubric.Tests.TestRules.Probabilistic;

[DependsOn(typeof(DepTestAttrRule))]
internal class DepTestAttrRule2 : DepTestAttrRule
{
    public DepTestAttrRule2(double expected, bool flagValue = true) : base(expected, flagValue)
    { 
    }

}