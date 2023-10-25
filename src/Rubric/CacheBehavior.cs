namespace Rubric;

/// <summary>
///   An enumeration specifying the level of available predicate caching.
/// </summary>
public enum CacheBehavior
{
  /// <summary>
  ///   Do not cache this predicate result.
  /// </summary>
  None,
  /// <summary>
  ///   Execute the predicate once per input object.
  /// </summary>
  PerInput,
  /// <summary>
  /// .Execute the predicate only once per engine execution.
  /// </summary>
  PerExecution
}