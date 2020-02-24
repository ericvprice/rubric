using RulesEngine.Rules.Async;
using System.Threading.Tasks;

namespace RulesEngine.Tests.DependencyRules.TypeAttribute
{
    internal class DepTestAsyncRule : AsyncRule<TestInput, TestOutput>
    {
        private readonly bool _expected;

        private readonly bool _flagValue;

        public DepTestAsyncRule(bool expected, bool flagValue = true)
        {
            _expected = expected;
            _flagValue = flagValue;
        }

        public override Task Apply(IEngineContext context, TestInput input, TestOutput output)
        {
            input.InputFlag = output.TestFlag = _flagValue;
            return Task.CompletedTask;
        }

        public override Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output)
            => Task.FromResult(_expected);
    }
}