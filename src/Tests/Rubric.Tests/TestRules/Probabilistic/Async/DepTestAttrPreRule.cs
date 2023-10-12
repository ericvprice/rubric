using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class DepTestAttrPreRule : Rule<TestInput>
{
    private readonly bool _flagValue;
    private readonly double _shouldApply;

    public DepTestAttrPreRule(double shouldApply, bool flagValue = true)
    {
        _flagValue = flagValue;
        _shouldApply = shouldApply;
    }

    public override Task Apply(IEngineContext context, TestInput obj, CancellationToken token)
    {
        obj.InputFlag = _flagValue;
        return Task.CompletedTask;
    }

    public override Task<double> DoesApply(IEngineContext context, TestInput obj, CancellationToken token)
      => Task.FromResult(_shouldApply);
}