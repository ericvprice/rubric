using System;
using RulesEngine.Rules;

namespace RulesEngine.Tests.TestRules
{
    public class TestExceptionPreRule : PreRule<TestInput>
    {
        public TestExceptionPreRule(bool onDoesApply)
        {
            OnDoesApply = onDoesApply;
        }

        public bool OnDoesApply { get; }

        public override void Apply(IEngineContext context, TestInput obj) 
        {
            obj.InputFlag = true;
            throw new Exception();
        }

        public override bool DoesApply(IEngineContext context, TestInput obj) => OnDoesApply ? throw new Exception() : true;
    }
}