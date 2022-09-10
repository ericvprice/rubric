using Rubric.Tests.TestAssembly;

namespace Rubric.Tests.TestAssembly2.TestRules;

internal class TestInputOutputRule : DefaultRule<TestAssemblyInput, TestAssemblyOutput2>
{
  public override void Apply(IEngineContext context, TestAssemblyInput input, TestAssemblyOutput2 output)
  {
  }

}
