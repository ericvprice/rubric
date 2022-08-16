using Rubric.Dependency;

namespace Rubric.Tests.TestRules.DepTestTypeAttribute;

[DependsOn("dep1")]
[DependsOn(typeof(DepTestPostRule))]
public class DepTestPostRule2 : Rule<TestOutput>
{
  private readonly bool _shouldApply;

  public DepTestPostRule2(bool shouldApply) => _shouldApply = shouldApply;

  public override void Apply(IEngineContext context, TestOutput obj) => obj.TestFlag = true;

  public override bool DoesApply(IEngineContext context, TestOutput obj) => _shouldApply;
}