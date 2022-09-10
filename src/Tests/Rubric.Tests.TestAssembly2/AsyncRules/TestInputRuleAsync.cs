using Rubric.Tests.TestAssembly;

namespace Rubric.Tests.TestAssembly2.AsyncRules;
internal class TestInputRuleAsync : DefaultAsyncRule<TestAssemblyInput>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, CancellationToken t)
   => Task.CompletedTask;

}
