namespace Rubric.Tests.TestRules.Async;

public class TestRule : Rubric.Rules.Async.Rule<TestInput, TestOutput>
{
  private readonly bool _shouldApply;

  public TestRule(bool shouldApply) => _shouldApply = shouldApply;

  public override Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
  {
    input.InputFlag = true;
    output.TestFlag = true;
    return Task.CompletedTask;
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
    => Task.FromResult(_shouldApply);
}