using System;
using System.Linq;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;
using RulesEngine.Tests.DependencyRules;
using RulesEngine.Tests.TestRules.Async;
using Xunit;

namespace RulesEngine.Tests
{
    public class AsyncRuleTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestDoesApply(bool expected)
        {
            var rule = new TestAsyncRule(expected);
            Assert.Equal(expected, await rule.DoesApply(null, null, null));
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestLambdaDoesApply(bool expected)
        {
            var rule = new LambdaAsyncRule<TestInput, TestOutput>("test", (c, i, o) => Task.FromResult(expected),
                                                                  (c, i, o) => Task.CompletedTask);
            Assert.Equal(expected, await rule.DoesApply(null, null, null));
        }

        [Fact]
        public void LambdaConstructor()
        {
            var rule = new LambdaAsyncRule<TestInput, TestOutput>("test", (c, i, o) => Task.FromResult(true),
                                                                  (c, i, o) => Task.CompletedTask,
                                                                  new[] { "dep1", "dep2" }, new[] { "prv1", "prv2" });
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
                () => new LambdaAsyncRule<TestInput, TestOutput>("test", null, (c, i, o) => Task.CompletedTask));
            Assert.Throws<ArgumentNullException>(
                () => new LambdaAsyncRule<TestInput, TestOutput>("test", (c, i, o) => Task.FromResult(true), null));
            Assert.Throws<ArgumentNullException>(
                () => new LambdaAsyncRule<TestInput, TestOutput>(null, (c, i, o) => Task.FromResult(true),
                                                                 (c, i, o) => Task.CompletedTask));
        }

        [Fact]
        public async Task TestDefaultDoesApply()
        {
            var rule = new TestDefaultAsyncRule();
            Assert.True(await rule.DoesApply(null, null, null));
        }

        [Fact]
        public void TestDependencies()
        {
            var rule = new DepTestAsyncRule(true);
            var dependencies = rule.Dependencies.ToList();
            Assert.Contains("dep1", dependencies);
            Assert.Contains("dep2", dependencies);
            Assert.Equal(2, dependencies.Count);
        }

        [Fact]
        public void TestProvides()
        {
            var rule = new DepTestAsyncRule(true);
            var provides = rule.Provides.ToList();
            Assert.Contains("dep3", provides);
            Assert.Contains(typeof(DepTestAsyncRule).FullName, provides);
            Assert.Equal(2, provides.Count);
        }
    }
}