using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.TestRules;

[ExcludeFromCodeCoverage]
internal class TestInputRule : DefaultRule<TestAssemblyInput>
{
  public override void Apply(IEngineContext context, TestAssemblyInput input)
  {
  }

}