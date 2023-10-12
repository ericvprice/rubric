using Rubric.Tests.TestRules;
using Rubric.Tests.TestRules.Async;
using DepTestAttrRule = Rubric.Tests.TestRules.Async.DepTestAttrRule;
using TestDefaultPreRule = Rubric.Tests.TestRules.Async.TestDefaultPreRule;
using TestPreRule = Rubric.Tests.TestRules.Async.TestPreRule;
using TestRule = Rubric.Tests.TestRules.TestRule;

namespace Rubric.Tests.Rules.Async;

public class RuleOfTTests
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
  public async Task DoesApply(bool expected)
  {
    var rule = new TestPreRule(expected);
    Assert.Equal(expected, await rule.DoesApply(null, null, default));
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public async Task LambdaDoesApply(bool expected)
  {
    var rule = new Rubric.Rules.Async.LambdaRule<TestOutput>("test", (_, _, _) => Task.FromResult(expected),
                                                    (_, _, _) => Task.CompletedTask);
    Assert.Equal(expected, await rule.DoesApply(null, null, default));
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new Rubric.Rules.Async.LambdaRule<TestInput>(
        "test",
        (_, _, _) => Task.FromResult(true),
        (_, _, _) => Task.CompletedTask,
        new[] { "dep1", "dep2" },
        new[] { "prv1", "prv2" }
    );
    Assert.Equal("test", rule.Name);
    Assert.Contains("dep1", rule.Dependencies);
    Assert.Contains("dep2", rule.Dependencies);
    Assert.Contains("prv1", rule.Provides);
    Assert.Contains("prv2", rule.Provides);
    Assert.NotNull(rule.Apply(null, null, default));
  }

  [Fact]
  public void LambdaConstructorException()
  {
    Assert.Throws<ArgumentNullException>(
    () => new Rubric.Rules.Async.LambdaRule<TestInput>("test", (IEngineContext _, TestInput _, CancellationToken _) => Task.FromResult(true), null));
    Assert.Throws<ArgumentException>(
    () => new Rubric.Rules.Async.LambdaRule<TestInput>(null, (IEngineContext _, TestInput _, CancellationToken _) => Task.FromResult(true),
                                            (IEngineContext _, TestInput _, CancellationToken _) => Task.CompletedTask));
    Assert.Throws<ArgumentException>(
    () => new Rubric.Rules.Async.LambdaRule<TestInput>("", (IEngineContext _, TestInput _, CancellationToken _) => Task.FromResult(true),
                                            (IEngineContext _, TestInput _, CancellationToken _) => Task.CompletedTask));

  }

  [Fact]
  public async Task TestDefaultDoesApply()
  {
    var rule = new TestDefaultPreRule();
    Assert.True(await rule.DoesApply(null, null, default));
  }

  [Fact]
  public void TestDependencies()
  {
    var rule = new DepTestPreRule(true);
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
