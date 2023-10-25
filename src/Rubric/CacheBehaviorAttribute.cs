namespace Rubric;

[AttributeUsage(AttributeTargets.Class)]
public class CacheBehaviorAttribute : Attribute
{
  public CacheBehaviorAttribute(CacheBehavior cacheBehavior, string key = null)
  {
    CacheBehavior = cacheBehavior;
  }

  public CacheBehavior CacheBehavior { get; }

  public string Key { get; } = null;
}

