using RulesEngine.Rules;

namespace RulesEngine.Tests.TestRules
{
    public class TestDefaultPreRule : DefaultPreRule<TestInput>
    {
        public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = true;
    }
}