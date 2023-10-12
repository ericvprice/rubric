using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly2.TestRules;

[ExcludeFromCodeCoverage]
internal class TestOutputRule : DefaultRule<TestAssemblyOutput2>
{
  public override void Apply(IEngineContext context, TestAssemblyOutput2 output)
  {
  }

}