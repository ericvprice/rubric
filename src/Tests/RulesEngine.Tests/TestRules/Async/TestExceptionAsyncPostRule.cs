using System;
using RulesEngine.Rules.Async;
using System.Threading.Tasks;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestExceptionAsyncPostRule : AsyncPostRule<TestOutput>
    {
        public TestExceptionAsyncPostRule(bool onDoesApply) 
            => OnDoesApply = onDoesApply;

        public bool OnDoesApply { get; }

        public override Task Apply(IEngineContext context, TestOutput obj) {
            obj.TestFlag = true;
            throw new Exception();
        }

        public override Task<bool> DoesApply(IEngineContext context, TestOutput obj) 
            => OnDoesApply ? throw new Exception() : Task.FromResult(true);
    }
}