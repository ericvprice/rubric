using Rubric.Dependency;
using Rubric.Rules.Async;

namespace Rubric.Tests.TestRules.Async;

[DependsOn(typeof(DepTestAttrPostRule))]
public class DepTestAttrPostRule2 : DepTestAttrPostRule
{

    public DepTestAttrPostRule2(bool shouldApply) : base(shouldApply) { }

}