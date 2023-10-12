using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.TestRules;


[ExcludeFromCodeCoverage]
internal class TestOutputRule : DefaultRule<TestAssemblyOutput>
{
  public override void Apply(IEngineContext context, TestAssemblyOutput output)
  {
  }

}