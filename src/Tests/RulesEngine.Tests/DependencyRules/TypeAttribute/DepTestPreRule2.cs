using RulesEngine.Dependency;
using RulesEngine.Rules;

namespace RulesEngine.Tests.DependencyRules.TypeAttribute
{
    [DependsOn(typeof(DepTestPreRule))]
    public class DepTestPreRule2 : PreRule<TestInput>
    {
        private readonly bool _shouldApply;

        private readonly bool _flagValue;

        public DepTestPreRule2(bool shouldApply, bool flagValue = true)
        {
            _flagValue = flagValue;
            _shouldApply = shouldApply;
        }

        public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = _flagValue;

        public override bool DoesApply(IEngineContext context, TestInput obj) => _shouldApply;
    }

}