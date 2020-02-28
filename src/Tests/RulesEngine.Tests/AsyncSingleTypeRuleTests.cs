using System;
using System.Linq;
using System.Threading.Tasks;
using RulesEngine.Rules.Async;
using RulesEngine.Tests.DependencyRules;
using RulesEngine.Tests.TestRules.Async;
using Xunit;

namespace RulesEngine.Tests
{
    public class AsyncSingleTypeRuleTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DoesApply(bool expected)
        {
            var rule = new TestAsyncPreRule(expected);
            Assert.Equal(expected, await rule.DoesApply(null, null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task LambdaDoesApply(bool expected)
        {
            var rule = new LambdaAsyncRule<TestOutput>("test", (c, i) => Task.FromResult(expected),
                                                           (c, i) => Task.CompletedTask);
            Assert.Equal(expected, await rule.DoesApply(null, null));
        }

        [Fact]
        public void LambdaConstructor()
        {
            var rule = new LambdaAsyncRule<TestInput>(
                "test",
                (c, i) => Task.FromResult(true),
                (c, i) => Task.CompletedTask,
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
                () => new LambdaAsyncRule<TestInput>("test", null, (c, i) => Task.CompletedTask));
            Assert.Throws<ArgumentNullException>(
                () => new LambdaAsyncRule<TestInput>("test", (c, i) => Task.FromResult(true), null));
            Assert.Throws<ArgumentException>(
                () => new LambdaAsyncRule<TestInput>(null, (c, i) => Task.FromResult(true),
                                                        (c, i) => Task.CompletedTask));
            Assert.Throws<ArgumentException>(
                () => new LambdaAsyncRule<TestInput>("", (c, i) => Task.FromResult(true),
                                                        (c, i) => Task.CompletedTask));

        }

        [Fact]
        public async Task TestDefaultDoesApply()
        {
            var rule = new TestDefaultAsyncPreRule();
            Assert.True(await rule.DoesApply(null, null));
        }

        [Fact]
        public void TestDependencies()
        {
            var rule = new DepTestAsyncPreRule(true);
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