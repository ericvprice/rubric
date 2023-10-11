using Rubric.Rules.Probabilistic.Async;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Rules.Probabilistic.Async;

public class AsyncUtilityRuleTests
{

  [Fact]
  public async Task DebugReturnsDebug()
  {
    const double expected =
#if DEBUG
    1;
#else
    0;
#endif
    Assert.Equal(expected, await new TestDebugRule<TestInput>().DoesApply(null, null, default));
    Assert.Equal(expected, await new TestDebugRule<TestInput, TestOutput>().DoesApply(null, null, null, default));
  }

  [Fact]
  public async Task NullReturnsFalse()
  {
    Assert.Equal(0, await new NullRule<TestInput>().DoesApply(null, null, default));
    Assert.Equal(0, await new NullRule<TestInput, TestOutput>().DoesApply(null, null, null, default));
    Assert.Equal(Task.CompletedTask, new NullRule<TestInput>().Apply(null, null, default));
    Assert.Equal(Task.CompletedTask, new NullRule<TestInput, TestOutput>().Apply(null, null, null, default));
  }

}