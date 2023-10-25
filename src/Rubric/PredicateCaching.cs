namespace Rubric;

public readonly struct PredicateCaching
{
  public PredicateCaching(CacheBehavior cacheBehavior, string cacheKey)
  {
    Behavior = cacheBehavior;
    Key = cacheKey;
  }

  public CacheBehavior Behavior { get; }

  public string Key { get; }
}