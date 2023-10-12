using Rubric.Rules.Probabilistic.Async;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Rules.Probabilistic.Async;

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
  public async Task DoesApply(double expected)
  {
    var rule = new TestPreRule(expected);
    Assert.Equal(expected, await rule.DoesApply(null, null, default));
  }

  [Theory]
  [InlineData(1)]
  [InlineData(0)]
  public async Task LambdaDoesApply(double expected)
  {
    var rule = new LambdaRule<TestOutput>("test", (_, _, _) => Task.FromResult(expected),
                                                    (_, _, _) => Task.CompletedTask);
    Assert.Equal(expected, await rule.DoesApply(null, null, default));
  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput>(
        "test",
        (_, _, _) => Task.FromResult(1D),
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
    () => new LambdaRule<TestInput>("test", (_, _, _) => Task.FromResult(1D), null));
    Assert.Throws<ArgumentException>(
    () => new LambdaRule<TestInput>(null, (_, _, _) => Task.FromResult(1D),
                                            (_, _, _) => Task.CompletedTask));
    Assert.Throws<ArgumentException>(
    () => new LambdaRule<TestInput>("", (_, _, _) => Task.FromResult(1D),
                                            (_, _, _) => Task.CompletedTask));

  }

  [Fact]
  public async Task DefaultDoesApply()
  {
    var rule = new TestDefaultPreRule();
    Assert.Equal(1, await rule.DoesApply(null, null, default));
  }
}
