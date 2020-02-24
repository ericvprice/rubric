using System.Linq;
using RulesEngine.Rules;
using Xunit;
using RulesEngine.Tests.TestRules;
using RulesEngine.Tests.DependencyRules;
using System;

namespace RulesEngine.Tests
{
    public class PostRuleTests
    {

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DoesApply(bool expected)
        {
            var rule = new TestPostRule(expected);
            Assert.Equal(expected, rule.DoesApply(null, null));
        }

        [Fact]
        public void LambdaConstructorException()
        {
            Assert.Throws<ArgumentNullException>(() => new LambdaPostRule<TestOutput>("test", null, (c, o) => { }));
            Assert.Throws<ArgumentNullException>(() => new LambdaPostRule<TestOutput>("test", (c, o) => true, null));
            Assert.Throws<ArgumentNullException>(() => new LambdaPostRule<TestOutput>(null, (c, o) => true, (c, o) => { }));
        }

        [Fact]
        public void LambdaConstructor()
        {
            var rule = new LambdaPostRule<TestOutput>("test", (c, o) => true, (c, o) => { }, new[] { "dep1", "dep2" }, new[] { "prv1", "prv2" });
            Assert.Equal("test", rule.Name);
            Assert.Contains("dep1", rule.Dependencies);
            Assert.Contains("dep2", rule.Dependencies);
            Assert.Contains("prv1", rule.Provides);
            Assert.Contains("prv2", rule.Provides);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LambdaDoesApply(bool expected)
        {
            var rule = new LambdaPostRule<TestInput>("test", (c, i) => expected, (c, i) => { });
            Assert.Equal(expected, rule.DoesApply(null, null));
        }

        [Fact]
        public void DefaultDoesApply()
        {
            var rule = new TestDefaultPostRule();
            Assert.True(rule.DoesApply(null, null));
        }

        [Fact]
        public void Dependencies()
        {
            var rule = new DepTestPostRule(true);
            var dependencies = rule.Dependencies.ToList();
            Assert.Contains("dep1", dependencies);
            Assert.Contains("dep2", dependencies);
            Assert.Equal(2, dependencies.Count);
        }

        [Fact]
        public void Provides()
        {
            var rule = new DepTestPostRule(true);
            var provides = rule.Provides.ToList();
            Assert.Contains("dep3", provides);
            Assert.Contains(typeof(DepTestPostRule).FullName, provides);
            Assert.Equal(2, provides.Count);
        }

    }
}
