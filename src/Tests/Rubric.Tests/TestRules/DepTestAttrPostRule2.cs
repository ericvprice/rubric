using Rubric.Dependency;

namespace Rubric.Tests.TestRules;

[DependsOn(typeof(DepTestAttrPostRule))]
public class DepTestAttrPostRule2 : DepTestAttrPostRule
{
    public DepTestAttrPostRule2(bool shouldApply) : base(shouldApply) { }
}