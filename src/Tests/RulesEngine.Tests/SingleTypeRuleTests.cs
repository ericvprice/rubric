using RulesEngine.Rules;
using RulesEngine.Tests.DependencyRules;
using RulesEngine.Tests.TestRules;
using Xunit;

namespace RulesEngine.Tests
{
    public class SingleTypeRuleTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DoesApply(bool expected)
        {
            var rule = new TestPreRule(expected);
            Assert.Equal(expected, rule.DoesApply(null, null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LambdaDoesApply(bool expected)
        {
            var rule = new LambdaRule<TestInput>("test", (c, i) => expected, (c, i) => { });
            Assert.Equal(expected, rule.DoesApply(null, null));
        }

        [Fact]
        public void DefaultDoesApply()
        {
            var rule = new TestDefaultPreRule();
            Assert.True(rule.DoesApply(null, null));
        }

        [Fact]
        public void Dependencies()
        {
            var rule = new DepTestPreRule(true);
            var dependencies = rule.Dependencies.ToList();
            Assert.Contains("dep1", dependencies);
            Assert.Contains("dep2", dependencies);
            Assert.Equal(2, dependencies.Count);
        }

        [Fact]
        public void LambdaConstructor()
        {
            var rule = new LambdaRule<TestInput>("test", (c, i) => true, (c, i) => { }, new[] { "dep1", "dep2" },
                                                    new[] { "prv1", "prv2" });
            Assert.Equal("test", rule.Name);
            Assert.Contains("dep1", rule.Dependencies);
            Assert.Contains("dep2", rule.Dependencies);
            Assert.Contains("prv1", rule.Provides);
            Assert.Contains("prv2", rule.Provides);
        }

        [Fact]
        public void LambdaConstructorException()
        {
            Assert.Throws<ArgumentNullException>(() => new LambdaRule<TestInput>("test", null, (c, i) => { }));
            Assert.Throws<ArgumentNullException>(() => new LambdaRule<TestInput>("test", (c, i) => true, null));
            Assert.Throws<ArgumentException>(
                () => new LambdaRule<TestInput>(null, (c, i) => true, (c, i) => { }));
        }

        [Fact]
        public void Provides()
        {
            var rule = new DepTestPreRule(true);
            var provides = rule.Provides.ToList();
            Assert.Contains("dep3", provides);
            Assert.Contains(typeof(DepTestPreRule).FullName, provides);
            Assert.Equal(2, provides.Count);
        }
    }
}