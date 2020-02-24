using RulesEngine.Rules;
using RulesEngine.Dependency;

namespace RulesEngine.Tests.DependencyRules.TypeAttribute
{
    [Provides("dep1")]
    public class DepTestPostRule : PostRule<TestOutput>
    {
        private readonly bool _shouldApply;

        public DepTestPostRule(bool shouldApply)
        {
            _shouldApply = shouldApply;
        }

        public override void Apply(IEngineContext context, TestOutput obj) => obj.TestFlag = true;

        public override bool DoesApply(IEngineContext context, TestOutput obj) => _shouldApply;
    }
}