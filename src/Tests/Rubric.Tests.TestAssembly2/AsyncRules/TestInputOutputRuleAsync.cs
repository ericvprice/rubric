using Rubric.Tests.TestAssembly;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly2.AsyncRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestInputOutputRuleAsync : DefaultAsyncRule<TestAssemblyInput, TestAssemblyOutput2>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, TestAssemblyOutput2 output, CancellationToken t)
   => Task.CompletedTask;

}