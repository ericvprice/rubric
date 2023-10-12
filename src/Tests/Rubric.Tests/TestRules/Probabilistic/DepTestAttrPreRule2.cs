using Rubric.Dependency;

namespace Rubric.Tests.TestRules.Probabilistic;

[DependsOn(typeof(DepTestAttrPreRule))]
public class DepTestAttrPreRule2 : DepTestAttrPreRule
{
  public DepTestAttrPreRule2(double shouldApply, bool flagValue = true) : base(shouldApply, flagValue) { }
}