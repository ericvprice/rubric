namespace Rubric.Tests.TestRules.DepTestTypeAttribute;

public class DepTestAsyncPostRule : AsyncRule<TestOutput>
{
  private readonly bool _shouldApply;

  public DepTestAsyncPostRule(bool shouldApply) => _shouldApply = shouldApply;

  public override Task Apply(IEngineContext context, TestOutput obj, CancellationToken token)
  {
    obj.TestFlag = true;
    return Task.CompletedTask;
  }

  public override Task<bool> DoesApply(IEngineContext context, TestOutput obj, CancellationToken token)
    => Task.FromResult(_shouldApply);
}