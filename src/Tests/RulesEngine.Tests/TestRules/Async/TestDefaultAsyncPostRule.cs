using RulesEngine.Rules.Async;
using System.Threading.Tasks;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestDefaultAsyncPostRule : DefaultAsyncPostRule<TestOutput>
    {
        public override string Name => nameof(TestDefaultPostRule);

        public override Task Apply(IEngineContext context, TestOutput obj) 
        {
            obj.TestFlag = true;
            return Task.CompletedTask;
        }
            
    }
}