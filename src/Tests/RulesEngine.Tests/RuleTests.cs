using System;
using System.Linq;
using RulesEngine.Rules;
using RulesEngine.Tests.DependencyRules;
using RulesEngine.Tests.TestRules;
using Xunit;

namespace RulesEngine.Tests
{
    public class RuleTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestDoesApply(bool expected)
        {
            var rule = new TestRule(expected);
            Assert.Equal(expected, rule.DoesApply(null, null, null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestLambdaDoesApply(bool expected)
        {
            var rule = new LambdaRule<TestInput, TestOutput>("test", (c, i, o) => expected, (c, i, o) => { });
            Assert.Equal(expected, rule.DoesApply(null, null, null));
        }

        [Fact]
        public void LambdaConstructor()
        {
            var rule = new LambdaRule<TestInput, TestOutput>("test", (c, i, o) => true, (c, i, o) => { },
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
                () => new LambdaRule<TestInput, TestOutput>("test", null, (c, i, o) => { }));
            Assert.Throws<ArgumentNullException>(
                () => new LambdaRule<TestInput, TestOutput>("test", (c, i, o) => true, null));
            Assert.Throws<ArgumentException>(
                () => new LambdaRule<TestInput, TestOutput>(null, (c, i, o) => true, (c, i, o) => { }));
        }

        [Fact]
        public void TestDefaultDoesApply()
        {
            var rule = new TestDefaultRule();
            Assert.True(rule.DoesApply(null, null, null));
        }

        [Fact]
        public void TestDependencies()
        {
            var rule = new DepTestRule(true);
            var dependencies = rule.Dependencies.ToList();
            Assert.Contains("dep1", dependencies);
            Assert.Contains("dep2", dependencies);
            Assert.Equal(2, dependencies.Count);
        }

        [Fact]
        public void TestProvides()
        {
            var rule = new DepTestRule(true);
            var provides = rule.Provides.ToList();
            Assert.Contains("dep3", provides);
            Assert.Contains(typeof(DepTestRule).FullName, provides);
            Assert.Equal(2, provides.Count);
        }
    }
}