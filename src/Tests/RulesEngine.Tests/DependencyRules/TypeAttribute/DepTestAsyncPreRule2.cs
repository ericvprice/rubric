using System.Threading.Tasks;
using RulesEngine.Dependency;
using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.DependencyRules.TypeAttribute
{
    [DependsOn(typeof(DepTestAsyncPreRule))]
    public class DepTestAsyncPreRule2 : AsyncRule<TestInput>
    {
        private readonly bool _flagValue;
        private readonly bool _shouldApply;

        public DepTestAsyncPreRule2(bool shouldApply, bool flagValue = true)
        {
            _flagValue = flagValue;
            _shouldApply = shouldApply;
        }

        public override Task Apply(IEngineContext context, TestInput obj)
        {
            obj.InputFlag = _flagValue;
            return Task.CompletedTask;
        }

        public override Task<bool> DoesApply(IEngineContext context, TestInput obj)
            => Task.FromResult(_shouldApply);
    }
}