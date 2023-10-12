using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.TestRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestInputOutputRule : DefaultRule<TestAssemblyInput, TestAssemblyOutput>
{
  public override void Apply(IEngineContext context, TestAssemblyInput input, TestAssemblyOutput output)
  {
  }

}
