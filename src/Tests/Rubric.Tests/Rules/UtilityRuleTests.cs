using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rubric.Tests.TestRules;

namespace Rubric.Tests.Rules
{
  public  class UtilityRuleTests
  {

    [Fact]
    public void DebugReturnsDebug()
    {
      var expected =
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
}
