using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class DepTestAttrPostRule : Rule<TestOutput>
{
    private readonly double _shouldApply;

    public DepTestAttrPostRule(double shouldApply) => _shouldApply = shouldApply;

    public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
    {
        obj.TestFlag = true;
        return Task.CompletedTask;
    }

    public override Task<double> DoesApply(IEngineContext context, TestOutput obj, CancellationToken token)
      => Task.FromResult(_shouldApply);
}