using Rubric.Rules.Async;

namespace Rubric.Tests.TestRules.Async;

public class DepTestAttrPreRule : Rule<TestInput>
{
    private readonly bool _flagValue;
    private readonly bool _shouldApply;

    public DepTestAttrPreRule(bool shouldApply, bool flagValue = true)
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