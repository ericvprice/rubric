using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly.AsyncRules;

[ExcludeFromCodeCoverage]
internal class TestInputRuleAsync : DefaultAsyncRule<TestAssemblyInput>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, CancellationToken t)
   => Task.CompletedTask;

}
