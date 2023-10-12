using Rubric.Engines.Default;

namespace Rubric.Engines.Probabilistic.Default;

public abstract class BaseProbabilisticRuleEngine : BaseRuleEngine
{
    public Random Random { get; }

    protected BaseProbabilisticRuleEngine()
    {
        Random = new Random((int)DateTime.Now.Ticks);
    }
}
