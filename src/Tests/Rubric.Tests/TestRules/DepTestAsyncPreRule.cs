using Rubric.Dependency;

namespace Rubric.Tests.TestRules;

[DependsOn("dep1")]
[DependsOn("dep2")]
[Provides("dep3")]
public class DepTestAsyncPreRule : Rubric.Rules.Async.Rule<TestInput>
{
  private readonly bool _flagValue;
  private readonly bool _shouldApply;

  public DepTestAsyncPreRule(bool shouldApply, bool flagValue = true)
  {
    _flagValue = flagValue;
    _shouldApply = shouldApply;
  }

  public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
  {
    obj.InputFlag = _flagValue;
    return Task.CompletedTask;
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput obj, CancellationToken token)
    => Task.FromResult(_shouldApply);
}