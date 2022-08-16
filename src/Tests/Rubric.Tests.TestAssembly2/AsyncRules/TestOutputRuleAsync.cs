namespace Rubric.Tests.TestAssembly2.TestRules;

internal class TestOutputRuleAsync : DefaultAsyncRule<TestAssemblyOutput2>
{
  public override Task Apply(IEngineContext context, TestAssemblyOutput2 input, CancellationToken t)
   => Task.CompletedTask;

}
