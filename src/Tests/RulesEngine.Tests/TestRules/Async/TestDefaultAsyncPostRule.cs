using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestDefaultAsyncPostRule : DefaultAsyncRule<TestOutput>
    {
        public override string Name => nameof(TestDefaultPostRule);

        public override Task Apply(IEngineContext context, TestOutput obj)
        {
            obj.TestFlag = true;
            return Task.CompletedTask;
        }
    }
}