using System;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;

namespace RulesEngine.Tests.TestRules.Async
{
    public class TestExceptionAsyncPreRule : AsyncPreRule<TestInput>
    {
        public TestExceptionAsyncPreRule(bool onDoesApply)
            => OnDoesApply = onDoesApply;

        public bool OnDoesApply { get; }

        public override Task Apply(IEngineContext context, TestInput obj)
        {
            obj.InputFlag = true;
            throw new Exception();
        }

        public override Task<bool> DoesApply(IEngineContext context, TestInput obj)
            => OnDoesApply ? throw new Exception() : Task.FromResult(true);
    }
}