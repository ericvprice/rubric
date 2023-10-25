using Rubric.Engines.Implementation;

namespace Rubric.Engines.Probabilistic.Implementation;

/// <summary>
///   Base class for probabilistic rule engines.
/// </summary>
public abstract class BaseProbabilisticRuleEngine : BaseRuleEngine
{
  /// <summary>
  ///   Random number source.
  /// </summary>
  public Random Random { get; } = new((int)DateTime.Now.Ticks);
}