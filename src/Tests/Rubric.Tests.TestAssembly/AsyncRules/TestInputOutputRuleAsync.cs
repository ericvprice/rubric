using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.AsyncRules;

[ExcludeFromCodeCoverage]
// ReSharper disable once UnusedType.Global
internal class TestInputOutputRuleAsync : DefaultAsyncRule<TestAssemblyInput, TestAssemblyOutput>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, TestAssemblyOutput output, CancellationToken t)
   => Task.CompletedTask;

}