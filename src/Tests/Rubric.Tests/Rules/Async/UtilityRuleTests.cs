using Rubric.Tests.TestRules.Async;

namespace Rubric.Tests.Rules.Async;

public  class AsyncUtilityRuleTests
{

  [Fact]
  public async Task DebugReturnsDebug()
  {
    const bool expected =
      #if DEBUG
          true;
      #else
      false;
    #endif
    Assert.Equal(expected, await new TestAsyncDebugRule<TestInput>().DoesApply(null, null, default));
    Assert.Equal(expected, await new TestAsyncDebugRule<TestInput, TestOutput>().DoesApply(null, null, null, default));
  }

  [Fact]
  public async Task NullReturnsFalse()
  {
    Assert.False(await new AsyncNullRule<TestInput>().DoesApply(null, null, default));
    Assert.False(await new AsyncNullRule<TestInput, TestOutput>().DoesApply(null, null, null, default));
    Assert.Equal(Task.CompletedTask, new AsyncNullRule<TestInput>().Apply(null, null, default));
    Assert.Equal(Task.CompletedTask, new AsyncNullRule<TestInput, TestOutput>().Apply(null, null, null, default));
  }

}