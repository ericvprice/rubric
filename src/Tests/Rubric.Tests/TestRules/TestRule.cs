using Rubric.Rules;

namespace Rubric.Tests.TestRules;

internal class TestRule : Rule<TestInput, TestOutput>
{
  private readonly bool _expected;

  private readonly bool _flagValue;

  public TestRule(bool expected, bool flagValue = true, PredicateCaching caching = default)
  {
    _expected = expected;
    _flagValue = flagValue;
    CacheBehavior = caching;
  }

  /// <inheritdoc />
  public override PredicateCaching CacheBehavior { get; }

  public override void Apply(IEngineContext context, TestInput input, TestOutput output)
    => input.InputFlag = output.TestFlag = _flagValue;

  public override bool DoesApply(IEngineContext context, TestInput input, TestOutput output)
    => _expected;
}