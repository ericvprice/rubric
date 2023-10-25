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
                        .OfType<CacheBehaviorAttribute>().ToArray();
          if(!attr.Any()) t.BaseType.GetPredicateCaching();
          var last = attr.LastOrDefault();
          var behavior = last?.CacheBehavior ?? CacheBehavior.None;
          var key = last?.Key ?? t.FullName;
          return new(behavior, key);
        });
  
}