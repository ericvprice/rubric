using Rubric.Tests.TestAssembly;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly2.TestRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestInputRule : DefaultRule<TestAssemblyInput>
{
  public override void Apply(IEngineContext context, TestAssemblyInput output)
  {
  }

}