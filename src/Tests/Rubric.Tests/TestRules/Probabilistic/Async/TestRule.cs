using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Tests.TestRules.Probabilistic.Async;

internal class TestRule : Rule<TestInput, TestOutput>
{
  private readonly double _expected;

  private readonly bool _flagValue;

  public TestRule(double expected, bool flagValue = true)
  {
    _expected = expected;
    _flagValue = flagValue;
  }

  public override Task Apply(IEngineContext context, TestInput input, TestOutput output, CancellationToken t)
  {
    input.InputFlag = output.TestFlag = _flagValue;
    return Task.CompletedTask;
  }

  public override Task<double> DoesApply(IEngineContext context, TestInput input, TestOutput output, CancellationToken t  )
    => Task.FromResult(_expected);
}