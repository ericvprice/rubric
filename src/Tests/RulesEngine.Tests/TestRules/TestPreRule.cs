using RulesEngine.Rules;

namespace RulesEngine.Tests.TestRules
{

    public class TestPreRule : PreRule<TestInput>
    {
        private readonly bool _shouldApply;
        
        private readonly bool _flagValue;

        public TestPreRule(bool shouldApply, bool flagValue = true) {
            _flagValue = flagValue;
            _shouldApply = shouldApply;
        }

        public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = _flagValue;

        public override bool DoesApply(IEngineContext context, TestInput obj) => _shouldApply;
    }
}