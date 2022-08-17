namespace Rubric.Tests.TestAssembly.AsyncRules;
internal class TestOutputRuleAsync : DefaultAsyncRule<TestAssemblyOutput>
{
  public override Task Apply(IEngineContext context, TestAssemblyOutput input, CancellationToken t)
   => Task.CompletedTask;

}
