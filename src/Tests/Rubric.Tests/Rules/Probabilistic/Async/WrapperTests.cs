using Rubric;
using Rubric.Tests.TestRules.Probabilistic.Async;

namespace Rubric.Tests.Rules.Probabilistic.Async;

public class WrapperTests
{

  [Fact]
  public async void WrapperOfT()
  {
    var sync = new TestRules.Probabilistic.TestPreRule(1);
    var async = sync.WrapAsync();
    Assert.Contains(nameof(TestPreRule), async.Name);
    Assert.Contains("wrapped", async.Name);
    var testInput = new TestInput();
    Assert.Equal(sync.Dependencies, async.Dependencies);
    Assert.Equal(sync.Provides, async.Provides);
    Assert.StartsWith(sync.Name, async.Name);
    Assert.Equal(sync.DoesApply(null, testInput), await async.DoesApply(null, testInput, default));
    await async.Apply(null, testInput, default);
    Assert.True(testInput.InputFlag);
  }

  [Fact]
  public async void WrapperOfTInTOut()
  {
    var sync = new TestRules.Probabilistic.TestRule(1);
    var async = sync.WrapAsync();
    Assert.Contains("TestRule", async.Name);
    Assert.Contains("wrapped", async.Name);
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
  }
}