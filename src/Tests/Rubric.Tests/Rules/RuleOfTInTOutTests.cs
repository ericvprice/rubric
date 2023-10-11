using Rubric.Rules;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules;

public class RuleOfTInTOutTests
{

  [Fact]
  public void Name()
  {
    var rule = new TestRule(true);
    Assert.EndsWith(nameof(TestRule), rule.Name);
  }


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
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _) => expected, (_, _, _) => { });
    Assert.Equal(expected, rule.DoesApply(null, null, null));
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _) => true, (_, _, _) => { },
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
        () => new LambdaRule<TestInput, TestOutput>("test", (_, _, _) => true, null));
    Assert.Throws<ArgumentException>(
        () => new LambdaRule<TestInput, TestOutput>(null, (_, _, _) => true, (_, _, _) => { }));
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
    Assert.Equal(3, provides.Count);
  }
}