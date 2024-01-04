namespace Rubric.Extensions.Serialization;

/// <summary>
///     Json serialization shim for predicate caching.
/// </summary>
public class PredicateCachingJson
{
    /// <summary>
    ///     The caching behavior.
    /// </summary>
    /// <value>The caching behavior.</value>
    public CacheBehavior Behavior { get; set; }

    /// <summary>
    ///     The cache key to use.
    /// </summary>
    /// <value>The cache key to use.</value>
    public string Key { get; set; }
}
