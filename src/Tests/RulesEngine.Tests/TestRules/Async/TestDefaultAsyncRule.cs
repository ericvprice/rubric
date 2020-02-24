using RulesEngine.Rules.Async;
using System.Threading.Tasks;

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