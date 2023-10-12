using Rubric.Tests.TestAssembly;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Tests.TestAssembly2.AsyncRules;

[ExcludeFromCodeCoverage]
internal class TestInputRuleAsync : DefaultAsyncRule<TestAssemblyInput>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, CancellationToken t)
   => Task.CompletedTask;

}
