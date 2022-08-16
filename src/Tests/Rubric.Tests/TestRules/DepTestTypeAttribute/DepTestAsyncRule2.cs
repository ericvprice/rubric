using Rubric.Dependency;

namespace Rubric.Tests.TestRules.DepTestTypeAttribute;

[DependsOn(typeof(DepTestAsyncRule))]
internal class DepTestAsyncRule2 : AsyncRule<TestInput, TestOutput>
{
  private readonly bool _expected;

  private readonly bool _flagValue;

  public DepTestAsyncRule2(bool expected, bool flagValue = true)
  {
    _expected = expected;
    _flagValue = flagValue;
  }

  public override Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
  {
    input.InputFlag = output.TestFlag = _flagValue;
    return Task.CompletedTask;
  }

  public override Task<bool> DoesApply(IEngineContext context, TestInput input, TestOutput output, CancellationToken token)
    => Task.FromResult(_expected);
}