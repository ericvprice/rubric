using Rubric.Dependency;

namespace Rubric.Tests.DependencyRules;

[DependsOn("dep1")]
[DependsOn("dep2")]
[Provides("dep3")]
public class DepTestPreRule : Rule<TestInput>
{
  private readonly bool _flagValue;
  private readonly bool _shouldApply;

  public DepTestPreRule(bool shouldApply, bool flagValue = true)
  {
    _flagValue = flagValue;
    _shouldApply = shouldApply;
  }

  public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = _flagValue;

  public override bool DoesApply(IEngineContext context, TestInput obj) => _shouldApply;
}