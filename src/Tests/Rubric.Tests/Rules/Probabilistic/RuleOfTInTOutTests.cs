using Rubric.Rules.Probabilistic;
using Rubric.Tests.TestRules.Probabilistic;

namespace Rubric.Tests.Rules.Probabilistic;

public class RuleOfTInTOutTests
{

  [Fact]
  public void Name()
  {
    var rule = new TestRule(1);
    Assert.EndsWith(nameof(TestRule), rule.Name);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(0)]
  public void TestDoesApply(double expected)
  {
    var rule = new TestRule(expected);
    Assert.Equal(expected, rule.DoesApply(null, null, null));
  }

  [Theory]
  [InlineData(1)]
  [InlineData(0)]
  public void TestLambdaDoesApply(double expected)
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _) => expected, (_, _, _) => { });
    Assert.Equal(expected, rule.DoesApply(null, null, null));
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _) => 1, (_, _, _) => { },
                                                     new[] { "dep1", "dep2" }, new[] { "prv1", "prv2" });
    Assert.Equal("test", rule.Name);
    Assert.Contains("dep1", rule.Dependencies);
    Assert.Contains("dep2", rule.Dependencies);
    Assert.Contains("prv1", rule.Provides);
    Assert.Contains("prv2", rule.Provides);
    rule.Apply(null, null, null);
  }


  [Fact]
  public void LambdaConstructorException()
  {
    Assert.Throws<ArgumentNullException>(
        () => new LambdaRule<TestInput, TestOutput>("test", null, (_, _, _) => { }));
    Assert.Throws<ArgumentNullException>(
        () => new LambdaRule<TestInput, TestOutput>("test", (_, _, _) => 1, null));
    Assert.Throws<ArgumentException>(
        () => new LambdaRule<TestInput, TestOutput>(null, (_, _, _) => 1, (_, _, _) => { }));
  }

  [Fact]
  public void TestDefaultDoesApply()
  {
    var rule = new TestDefaultRule();
    Assert.Equal(1, rule.DoesApply(null, null, null));
  }
}