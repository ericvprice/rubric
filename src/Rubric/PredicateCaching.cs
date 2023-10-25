namespace Rubric;

/// <summary>
///   Struct directing engines how to cache predicate results.
/// </summary>
public readonly struct PredicateCaching : IEquatable<PredicateCaching>
{
  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="cacheBehavior">The caching behavior.</param>
  /// <param name="cacheKey">The cache key.</param>
  public PredicateCaching(CacheBehavior cacheBehavior, string cacheKey)
  {
    Behavior = cacheBehavior;
    Key = cacheKey;
  }

  /// <summary>
  ///   The desired caching behavior.
  /// </summary>
  public CacheBehavior Behavior { get; }

  /// <summary>
  ///   The key to use.
  /// </summary>
  public string Key { get; }

  /// <inheritdoc />
  public override bool Equals(object obj)
    => obj is PredicateCaching caching && Equals(caching);

  /// <inheritdoc />
  public override int GetHashCode() => Key.GetHashCode(StringComparison.Ordinal) * 33 + Behavior.GetHashCode();

  /// <summary>
  ///   Equality comparison
  /// </summary>
  /// <param name="left">LHS</param>
  /// <param name="right">RHS</param>
  /// <returns>Whether the two are equal.</returns>
  public static bool operator ==(PredicateCaching left, PredicateCaching right) => left.Equals(right);

  /// <summary>
  ///   Inequality comparison
  /// </summary>
  /// <param name="left">LHS</param>
  /// <param name="right">RHS</param>
  /// <returns>Whether the two are equal.</returns>
  public static bool operator !=(PredicateCaching left, PredicateCaching right) => !(left == right);

  /// <inheritdoc />
  public bool Equals(PredicateCaching other) => GetHashCode() == other.GetHashCode();
}