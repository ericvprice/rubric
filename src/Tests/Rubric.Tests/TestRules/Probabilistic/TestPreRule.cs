using Rubric.Rules.Probabilistic;

namespace Rubric.Tests.TestRules.Probabilistic;

public class TestPreRule : Rule<TestInput>
{
  private readonly bool _flagValue;
  private readonly double _shouldApply;

  public TestPreRule(double shouldApply, bool flagValue = true, PredicateCaching caching = default)
  {
    _flagValue = flagValue;
    _shouldApply = shouldApply;
    CacheBehavior = caching;
  }

  /// <inheritdoc />
  public override PredicateCaching CacheBehavior { get; }

  /// <inheritdoc />
  public override void Apply(IEngineContext context, TestInput obj) => obj.InputFlag = _flagValue;

  /// <inheritdoc />
  public override double DoesApply(IEngineContext context, TestInput obj) => _shouldApply;
}