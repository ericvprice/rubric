using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestDefaultAsyncRule : DefaultAsyncRule<TestInput, TestOutput>
    {
        public override Task Apply(IEngineContext context, TestInput input, TestOutput output)
        {
            input.InputFlag = true;
            output.TestFlag = true;
            return Task.CompletedTask;
        }
    }
}