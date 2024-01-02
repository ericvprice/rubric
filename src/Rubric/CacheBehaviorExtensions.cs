using System.Collections.Concurrent;

namespace Rubric;

internal static class CacheBehaviorExtensions
{
  private static readonly ConcurrentDictionary<Type, PredicateCaching> _behaviorTypeCache = new();

  public static PredicateCaching GetPredicateCaching(this Type type)
    => _behaviorTypeCache.GetOrAdd(
      type,
      t =>
      {
        var attr = t.GetCustomAttributes(false)
                    .OfType<PredicateCachingAttribute>().ToArray();
        if (attr.Length == 0)
        {
          return t.BaseType?.GetPredicateCaching()
                 ?? default;
        }
        var last = attr.Last();
        var behavior = last.CacheBehavior;
        var key = last.Key ?? t.FullName;
        return new(behavior, key);
      });
}