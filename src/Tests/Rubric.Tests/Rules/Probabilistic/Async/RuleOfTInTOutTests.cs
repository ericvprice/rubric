using Rubric.Rules.Probabilistic.Async;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Rules.Probabilistic.Async;

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
  public async Task TestDoesApply(double expected)
  {
    var rule = new TestRule(expected);
    Assert.Equal(expected, await rule.DoesApply(null, null, null, default));
  }


  [Theory]
  [InlineData(1)]
  [InlineData(0)]
  public async Task TestLambdaDoesApply(double expected)
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _, _) => Task.FromResult(expected),
                                                          (_, _, _, _) => Task.CompletedTask);
    Assert.Equal(expected, await rule.DoesApply(null, null, null, default));

  }

  [Fact]
  public void LambdaConstructor()
  {
    var rule = new LambdaRule<TestInput, TestOutput>("test", (_, _, _, _) => Task.FromResult(1D),
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
    () => new LambdaRule<TestInput, TestOutput>("test", (IEngineContext _, TestInput _, TestOutput _, CancellationToken _) => Task.FromResult(1D), null));
    Assert.Throws<ArgumentNullException>(
    () => new LambdaRule<TestInput, TestOutput>(null, (IEngineContext _, TestInput _, TestOutput _, CancellationToken _) => Task.FromResult(1D),
                                                     (IEngineContext _, TestInput _, TestOutput _, CancellationToken _) => Task.CompletedTask));
  }

  [Fact]
  public async Task DefaultDoesApply()
  {
    var rule = new TestDefaultRule();
    Assert.Equal(1, await rule.DoesApply(null, null, null, default));
  }
}