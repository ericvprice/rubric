namespace Rubric.Tests.TestAssembly.TestRules;
internal class TestInputOutputRuleAsync : DefaultAsyncRule<TestAssemblyInput, TestAssemblyOutput>
{
  public override Task Apply(IEngineContext context, TestAssemblyInput input, TestAssemblyOutput output, CancellationToken t)
   => Task.CompletedTask;

}