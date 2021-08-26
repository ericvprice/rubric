using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestAsyncRule : AsyncRule<TestInput, TestOutput>
    {
        private readonly bool _shouldApply;

        public TestAsyncRule(bool shouldApply) => _shouldApply = shouldApply;

        public override Task Apply(IEngineContext context, TestInput input, TestOutput output)
        {
            input.InputFlag = true;
            output.TestFlag = true;
            return Task.CompletedTask;
        }

        public override Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output)
            => Task.FromResult(_shouldApply);
    }
}