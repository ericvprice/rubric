namespace Rubric.Tests.TestAssembly.AsyncRules;
internal class TestInputRuleAsync : DefaultAsyncRule<TestAssemblyInput>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, CancellationToken t)
   => Task.CompletedTask;

}
