using Rubric.Rules;

namespace Rubric.Tests.TestRules;

public class DepTestAttrPostRule : Rule<TestOutput>
{
    private readonly bool _shouldApply;

    public DepTestAttrPostRule(bool shouldApply) => _shouldApply = shouldApply;

    public override void Apply(IEngineContext context, TestOutput obj) => obj.TestFlag = true;

    public override bool DoesApply(IEngineContext context, TestOutput obj) => _shouldApply;
}