using Rubric.Rules;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules;

public class UtilityRuleTests
{

  [Fact]
  public void DebugReturnsDebug()
  {
    const bool expected =
#if DEBUG
          true;
#else
      false;
#endif
    Assert.Equal(expected, new TestDebugRule<TestInput>().DoesApply(null, null));
    Assert.Equal(expected, new TestDebugRule<TestInput, TestOutput>().DoesApply(null, null, null));
  }

  [Fact]
  public void NullReturnsFalse()
  {
    Assert.False(new NullRule<TestInput>().DoesApply(null, null));
    Assert.False(new NullRule<TestInput, TestOutput>().DoesApply(null, null, null));
  }

}