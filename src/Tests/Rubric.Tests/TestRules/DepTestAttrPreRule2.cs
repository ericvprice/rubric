using Rubric.Dependency;

namespace Rubric.Tests.TestRules;

[DependsOn(typeof(DepTestAttrPreRule))]
public class DepTestAttrPreRule2 : DepTestAttrPreRule
{

    public DepTestAttrPreRule2(bool shouldApply, bool flagValue = true) : base(shouldApply, flagValue) { }

}