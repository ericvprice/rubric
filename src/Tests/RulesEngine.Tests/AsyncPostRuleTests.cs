using System;
using System.Linq;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;
using RulesEngine.Tests.DependencyRules;
using RulesEngine.Tests.TestRules.Async;
using Xunit;

namespace RulesEngine.Tests
{
    public class AsyncPostRuleTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DoesApply(bool expected)
        {
            var rule = new TestAsyncPostRule(expected);
            Assert.Equal(expected, await rule.DoesApply(null, null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task LambdaDoesApply(bool expected)
        {
            var rule = new LambdaAsyncPostRule<TestOutput>("test", (c, i) => Task.FromResult(expected),
                                                           (c, i) => Task.CompletedTask);
            Assert.Equal(expected, await rule.DoesApply(null, null));
        }

        [Fact]
        public void LambdaConstructor()
        {
            var rule = new LambdaAsyncPostRule<TestOutput>(
                "test",
                (c, o) => Task.FromResult(true),
                (c, o) => Task.CompletedTask,
                new[] { "dep1", "dep2" },
                new[] { "prv1", "prv2" }
            );
            Assert.Equal("test", rule.Name);
            Assert.Contains("dep1", rule.Dependencies);
            Assert.Contains("dep2", rule.Dependencies);
            Assert.Contains("prv1", rule.Provides);
            Assert.Contains("prv2", rule.Provides);
        }

        [Fact]
        public void LambdaConstructorException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new LambdaAsyncPostRule<TestOutput>("test", null, (c, o) => Task.CompletedTask));
            Assert.Throws<ArgumentNullException>(
                () => new LambdaAsyncPostRule<TestOutput>("test", (c, o) => Task.FromResult(true), null));
            Assert.Throws<ArgumentNullException>(
                () => new LambdaAsyncPostRule<TestOutput>(null, (c, o) => Task.FromResult(true),
                                                          (c, o) => Task.CompletedTask));
        }

        [Fact]
        public async Task TestDefaultDoesApply()
        {
            var rule = new TestDefaultAsyncPostRule();
            Assert.True(await rule.DoesApply(null, null));
        }

        [Fact]
        public void TestDependencies()
        {
            var rule = new DepTestAsyncPostRule(true);
            var dependencies = rule.Dependencies.ToList();
            Assert.Contains("dep1", dependencies);
            Assert.Contains("dep2", dependencies);
            Assert.Equal(2, dependencies.Count);
        }

        [Fact]
        public void TestProvides()
        {
            var rule = new DepTestAsyncPostRule(true);
            var provides = rule.Provides.ToList();
            Assert.Contains("dep3", provides);
            Assert.Contains(typeof(DepTestAsyncPostRule).FullName, provides);
            Assert.Equal(2, provides.Count);
        }
    }
}