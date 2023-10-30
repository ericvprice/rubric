using Rubric.Rules.Async;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules.Async;

public class WrapperTests
{

  [Fact]
  public async void WrapperOfT()
  {
    var sync = new TestPreRule(true, true, new (CacheBehavior.PerExecution, "test"));

    var async = new AsyncRuleWrapper<TestInput>(sync);
    var testInput = new TestInput();
    Assert.Equal(sync.Dependencies, async.Dependencies);
    Assert.Equal(sync.Provides, async.Provides);
    Assert.StartsWith(sync.Name, async.Name);
    Assert.Equal(sync.DoesApply(null, testInput), await async.DoesApply(null, testInput, default));
    await async.Apply(null, testInput, default);
    Assert.True(testInput.InputFlag);
    Assert.Equal(CacheBehavior.PerExecution, async.CacheBehavior.Behavior);
    Assert.Equal("test", async.CacheBehavior.Key);
  }

  [Fact]
  public async void WrapperOfTInTOut()
  {
    var sync = new TestRule(true, true, new (CacheBehavior.PerExecution, "test"));
    var async = new AsyncRuleWrapper<TestInput, TestOutput>(sync);
    var testInput = new TestInput();
    var testOutput = new TestOutput();
    Assert.Equal(sync.Dependencies, async.Dependencies);
    Assert.Equal(sync.Provides, async.Provides);
    Assert.StartsWith(sync.Name, async.Name);
    Assert.Equal(sync.DoesApply(null, testInput, testOutput),
                  await async.DoesApply(null, testInput, testOutput, default));
    await async.Apply(null, testInput, testOutput, default);
    Assert.True(testOutput.TestFlag);
    Assert.True(testInput.InputFlag);
    Assert.Equal(CacheBehavior.PerExecution, async.CacheBehavior.Behavior);
    Assert.Equal("test", async.CacheBehavior.Key);

  }

}