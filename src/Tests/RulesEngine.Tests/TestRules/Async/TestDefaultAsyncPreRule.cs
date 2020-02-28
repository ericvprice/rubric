using System.Threading.Tasks;
using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestDefaultAsyncPreRule : DefaultAsyncRule<TestInput>
    {
        public override string Name => nameof(TestDefaultPostRule);

        public override Task Apply(IEngineContext context, TestInput obj)
        {
            obj.InputFlag = true;
            return Task.CompletedTask;
        }
    }
}