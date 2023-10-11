using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

public class TestPreHaltRule : DefaultRule<TestInput>
{
  public override Task Apply(IEngineContext context, TestInput input, CancellationToken token)
    => throw new EngineHaltException();

}