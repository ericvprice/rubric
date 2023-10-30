namespace Rubric;

/// <summary>
///   Declarative attribute for rule classes specifying predicate caching behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PredicateCachingAttribute : Attribute
{
  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="cacheBehavior">The caching behavior.</param>
  /// <param name="key">The key.</param>
  public PredicateCachingAttribute(CacheBehavior cacheBehavior, string key = null)
  {
    CacheBehavior = cacheBehavior;
    Key = key;
  }

  /// <summary>
  ///   The caching behavior to use.
  /// </summary>
  public CacheBehavior CacheBehavior { get; }

  /// <summary>
  ///   The key to use.
  /// </summary>
  public string Key { get; }
}