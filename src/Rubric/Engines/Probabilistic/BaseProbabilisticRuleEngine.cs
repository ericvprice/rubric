namespace Rubric.Engines;

public abstract class BaseProbabilisticRuleEngine : BaseRuleEngine
{
  public Random Random { get; }

  protected BaseProbabilisticRuleEngine()
  {
    Random = new Random((int)DateTime.Now.Ticks);
  }
}
