using System.Threading.Tasks;
using RulesEngine.Dependency;
using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.DependencyRules
{
    [DependsOn("dep1")]
    [DependsOn("dep2")]
    [Provides("dep3")]
    public class DepTestAsyncPostRule : AsyncRule<TestOutput>
    {
        private readonly bool _shouldApply;

        public DepTestAsyncPostRule(bool shouldApply) => _shouldApply = shouldApply;

        public override Task Apply(IEngineContext context, TestOutput obj)
        {
            obj.TestFlag = true;
            return Task.CompletedTask;
        }

        public override Task<bool> DoesApply(IEngineContext context, TestOutput obj)
            => Task.FromResult(_shouldApply);
    }
}