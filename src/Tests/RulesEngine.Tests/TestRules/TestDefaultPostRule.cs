using RulesEngine.Rules;

namespace RulesEngine.Tests.TestRules
{
    public class TestDefaultPostRule : DefaultPostRule<TestOutput>
    {
        public override string Name => nameof(TestDefaultPostRule);

        public override void Apply(IEngineContext context, TestOutput obj)
            => obj.TestFlag = true;

    }
}