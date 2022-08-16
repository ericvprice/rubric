using Rubric.Dependency;

namespace Rubric.Tests.TestRules;

[DependsOn("dep1")]
[DependsOn("dep2")]
[Provides("dep3")]
public class DepTestPostRule : Rule<TestOutput>
{
  private readonly bool _shouldApply;

  public DepTestPostRule(bool shouldApply) => _shouldApply = shouldApply;

  public override void Apply(IEngineContext context, TestOutput obj) => obj.TestFlag = true;

  public override bool DoesApply(IEngineContext context, TestOutput obj) => _shouldApply;
}