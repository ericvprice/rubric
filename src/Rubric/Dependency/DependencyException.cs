namespace Rubric.Dependency;

/// <summary>
///   Exception thrown when errors are encountered resolving dependencies.
/// </summary>
public class DependencyException : Exception
{
  /// <summary>
  ///   Default constructor
  /// </summary>
  public DependencyException() { }

  /// <summary>
  ///   Message constructor.
  /// </summary>
  /// <param name="message">The exception message.</param>
  public DependencyException(string message) : base(message) { }

  /// <summary>
  ///   Message and exception constructor
  /// </summary>
  /// <param name="message">The message</param>
  /// <param name="innerException">The inner exception</param>
  public DependencyException(string message, Exception innerException = null) : base(message, innerException) { }

  /// <summary>
  ///   A list of detailed error results.
  /// </summary>
  public IEnumerable<string> Details { get; set; } = new List<string>();
}