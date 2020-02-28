using RulesEngine.Rules.Async;
using RulesEngine.Tests.TestRules;
using Xunit;

namespace RulesEngine.Tests
{
    public class AsyncWrapperTests
    {


        [Fact]
        public async void SingleTypeWrapper()
        {
            var sync = new TestPreRule(true);
            var async = new AsyncRuleWrapper<TestInput>(sync);
            var testInput = new TestInput();
            Assert.Equal(sync.Dependencies, async.Dependencies);
            Assert.Equal(sync.Provides, async.Provides);
            Assert.StartsWith(sync.Name, async.Name);
            Assert.Equal(sync.DoesApply(null, testInput), await async.DoesApply(null, testInput));
            await async.Apply(null, testInput);
            Assert.True(testInput.InputFlag);
        }

        [Fact]
        public async void Wrapper()
        {
            var sync = new TestRule(true);
            var async = new AsyncRuleWrapper<TestInput, TestOutput>(sync);
            var testInput = new TestInput();
            var testOutput = new TestOutput();
            Assert.Equal(sync.Dependencies, async.Dependencies);
            Assert.Equal(sync.Provides, async.Provides);
            Assert.StartsWith(sync.Name, async.Name);
            Assert.Equal(sync.DoesApply(null, testInput, testOutput),
                         await async.DoesApply(null, testInput, testOutput));
            await async.Apply(null, testInput, testOutput);
            Assert.True(testOutput.TestFlag);
            Assert.True(testInput.InputFlag);
        }
    }
}