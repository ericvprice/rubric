using System.Threading.Tasks;
using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestAsyncPreRule : AsyncPreRule<TestInput>
    {
        private readonly bool _flagValue;
        private readonly bool _shouldApply;

        public TestAsyncPreRule(bool shouldApply, bool flagValue = true)
        {
            _shouldApply = shouldApply;
            _flagValue = flagValue;
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