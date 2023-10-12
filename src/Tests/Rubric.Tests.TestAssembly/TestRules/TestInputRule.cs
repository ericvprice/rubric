using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.TestRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestInputRule : DefaultRule<TestAssemblyInput>
{
  public override void Apply(IEngineContext context, TestAssemblyInput input)
  {
  }

}