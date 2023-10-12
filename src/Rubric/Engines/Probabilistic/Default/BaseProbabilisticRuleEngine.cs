using Rubric.Engines.Default;

namespace Rubric.Engines.Probabilistic.Default;

public abstract class BaseProbabilisticRuleEngine : BaseRuleEngine
{
    public Random Random { get; } = new((int)DateTime.Now.Ticks);
}
