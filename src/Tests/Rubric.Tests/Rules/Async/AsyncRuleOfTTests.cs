using Rubric.Tests.DependencyRules;
using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Rules.Async;

public class AsyncSingleTypeRuleTests
{
  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task DoesApply(bool expected)
  {
    var rule = new TestAsyncPreRule(expected);
    Assert.Equal(expected, await rule.DoesApply(null, null, default));
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task LambdaDoesApply(bool expected)
  {
    var rule = new LambdaAsyncRule<TestOutput>("test", (c, i, t) => Task.FromResult(expected),
                                                    (c, i, t) => Task.CompletedTask);
    Assert.Equal(expected, await rule.DoesApply(null, null, default));
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaAsyncRule<TestInput>(
        "test",
        (c, i, t) => Task.FromResult(true),
        (c, i, t) => Task.CompletedTask,
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
        () => new LambdaAsyncRule<TestInput>("test", (c, i, t) => Task.FromResult(true), null));
    Assert.Throws<ArgumentException>(
        () => new LambdaAsyncRule<TestInput>(null, (c, i, t) => Task.FromResult(true),
                                                (c, i, t) => Task.CompletedTask));
    Assert.Throws<ArgumentException>(
        () => new LambdaAsyncRule<TestInput>("", (c, i, t) => Task.FromResult(true),
                                                (c, i, t) => Task.CompletedTask));

  }

  [Fact]
  public async Task TestDefaultDoesApply()
  {
    var rule = new TestDefaultAsyncPreRule();
    Assert.True(await rule.DoesApply(null, null, default));
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
