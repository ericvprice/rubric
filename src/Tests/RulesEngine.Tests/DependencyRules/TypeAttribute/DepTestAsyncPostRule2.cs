using RulesEngine.Dependency;
using RulesEngine.Rules.Async;
using System.Threading.Tasks;

namespace RulesEngine.Tests.DependencyRules.TypeAttribute
{
    [DependsOn(typeof(DepTestAsyncPostRule))]
    public class DepTestAsyncPostRule2 : AsyncPostRule<TestOutput>
    {
        private readonly bool _shouldApply;

        public DepTestAsyncPostRule2(bool shouldApply)
        {
            _shouldApply = shouldApply;
        }

        public override Task Apply(IEngineContext context, TestOutput obj)
        {
            obj.TestFlag = true;
            return Task.CompletedTask;
        }

        public override Task<bool> DoesApply(IEngineContext context, TestOutput obj)
            => Task.FromResult(_shouldApply);
    }
}