using Rubric.Dependency;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

[DependsOn(typeof(DepTestAttrRule))]
internal class DepTestAttrRule2 : DepTestAttrRule
{

  public DepTestAttrRule2(double expected, bool flagValue = true) : base(expected, flagValue) { }

}