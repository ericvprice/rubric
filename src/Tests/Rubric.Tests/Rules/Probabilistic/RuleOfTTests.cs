using Rubric.Rules.Probabilistic;
using Rubric.Tests.TestRules.Probabilistic;

namespace Rubric.Tests.Rules.Probabilistic;

public class RuleOfTTests
{

  [Fact]
  public void Name()
  {
    var rule = new TestPreRule(1);
    Assert.EndsWith(nameof(TestPreRule), rule.Name);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(0)]
  public void DoesApply(double expected)
  {
    var rule = new TestPreRule(expected);
    Assert.Equal(expected, rule.DoesApply(null, null));
  }

  [Theory]
  [InlineData(1D)]
  [InlineData(0D)]
  public void LambdaDoesApply(double expected)
  {
    var rule = new LambdaRule<TestInput>("test", (_, _) => expected, (_, _) => { });
    Assert.Equal(expected, rule.DoesApply(null, null));
  }

  [Fact]
  public void DefaultDoesApply()
  {
    var rule = new TestDefaultPreRule();
    Assert.Equal(1, rule.DoesApply(null, null));
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput>("test", (_, _) => 1, (_, _) => { }, new[] { "dep1", "dep2" },
                                            new[] { "prv1", "prv2" });
    Assert.Equal("test", rule.Name);
    Assert.Contains("dep1", rule.Dependencies);
    Assert.Contains("dep2", rule.Dependencies);
    Assert.Contains("prv1", rule.Provides);
    Assert.Contains("prv2", rule.Provides);
    rule.Apply(null, null);
  }

  [Fact]
  public void LambdaConstructorException()
  {
    Assert.Throws<ArgumentNullException>(() => new LambdaRule<TestInput>("test", null, (_, _) => { }));
    Assert.Throws<ArgumentNullException>(() => new LambdaRule<TestInput>("test", (_, _) => 1, null));
    Assert.Throws<ArgumentException>(
        () => new LambdaRule<TestInput>(null, (_, _) => 1, (_, _) => { }));
  }
}
