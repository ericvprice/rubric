using System.Threading.Tasks;
using RulesEngine.Dependency;
using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.DependencyRules
{
    [DependsOn("dep1")]
    [DependsOn("dep2")]
    [Provides("dep3")]
    public class DepTestAsyncPreRule : AsyncPreRule<TestInput>
    {
        private readonly bool _flagValue;
        private readonly bool _shouldApply;

        public DepTestAsyncPreRule(bool shouldApply, bool flagValue = true)
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