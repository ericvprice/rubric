using Rubric.Rules.Async;
using Rubric.Tests.TestRules;
using Rubric.Tests.TestRules.Async;
using DepTestAttrRule = Rubric.Tests.TestRules.Async.DepTestAttrRule;
using TestDefaultRule = Rubric.Tests.TestRules.Async.TestDefaultRule;
using TestRule = Rubric.Tests.TestRules.Async.TestRule;

namespace Rubric.Tests.Rules.Async;

public class RuleOfTInTOutTests
{

  [Fact]
  public void Name()
  {
    var rule = new TestRules.TestRule(true);
    Assert.EndsWith(nameof(TestRules.TestRule), rule.Name);
  }


  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task TestDoesApply(bool expected)
  {
    var rule = new TestRule(expected);
    Assert.Equal(expected, await rule.DoesApply(null, null, null, default));
  }


  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task TestLambdaDoesApply(bool expected)
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _, _) => Task.FromResult(expected),
                                                          (_, _, _, _) => Task.CompletedTask);
    Assert.Equal(expected, await rule.DoesApply(null, null, null, default));

  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _, _) => Task.FromResult(true),
                                                          (_, _, _, _) => Task.CompletedTask,
                                                          new[] { "dep1", "dep2" }, new[] { "prv1", "prv2" });
    Assert.Equal("test", rule.Name);
    Assert.Contains("dep1", rule.Dependencies);
    Assert.Contains("dep2", rule.Dependencies);
    Assert.Contains("prv1", rule.Provides);
    Assert.Contains("prv2", rule.Provides);
    Assert.NotNull(rule.Apply(null, null, null, default));
  }

  [Fact]
  public void LambdaConstructorException()
  {
    Assert.Throws<ArgumentNullException>(
    () => new LambdaRule<TestInput, TestOutput>("test", (IEngineContext _, TestInput _, TestOutput _, CancellationToken _) => Task.FromResult(true), null));
    Assert.Throws<ArgumentNullException>(
    () => new LambdaRule<TestInput, TestOutput>(null, (IEngineContext _, TestInput _, TestOutput _, CancellationToken _) => Task.FromResult(true),
                                                     (IEngineContext _, TestInput _, TestOutput _, CancellationToken _) => Task.CompletedTask));
  }

  [Fact]
  public async Task TestDefaultDoesApply()
  {
    var rule = new TestDefaultRule();
    Assert.True(await rule.DoesApply(null, null, null, default));
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