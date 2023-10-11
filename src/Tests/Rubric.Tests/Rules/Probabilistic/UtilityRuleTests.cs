using Rubric.Rules.Probabilistic;
using Rubric.Tests.TestRules.Probabilistic;

namespace Rubric.Tests.Rules.Probabilistic;

public class UtilityRuleTests
{

  [Fact]
  public void DebugReturnsDebug()
  {
    const double expected =
#if DEBUG
      1;
#else
      0;
#endif
    Assert.Equal(expected, new TestDebugRule<TestInput>().DoesApply(null, null));
    Assert.Equal(expected, new TestDebugRule<TestInput, TestOutput>().DoesApply(null, null, null));
  }

  [Fact]
  public void NullReturnsFalse()
  {
    Assert.Equal(0, new NullRule<TestInput>().DoesApply(null, null));
    Assert.Equal(0, new NullRule<TestInput, TestOutput>().DoesApply(null, null, null));
  }

}