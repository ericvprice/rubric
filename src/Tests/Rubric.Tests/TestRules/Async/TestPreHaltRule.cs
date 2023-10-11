using Rubric.Rules.Async;

namespace Rubric.Tests.TestRules.Async;

public class TestPreHaltRule : DefaultAsyncRule<TestInput>
{
  public override Task Apply(IEngineContext context, TestInput input, CancellationToken token)
    => throw new EngineHaltException();

}