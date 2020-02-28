using RulesEngine.Rules;

namespace RulesEngine.Tests.TestRules
{
    public class TestDefaultRule : DefaultRule<TestInput, TestOutput>
    {
        public override void Apply(IEngineContext context, TestInput input, TestOutput output)
            => input.InputFlag = output.TestFlag = true;
    }


}