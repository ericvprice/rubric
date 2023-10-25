namespace Rubric.Dependency;

/// <summary>
///   Declaratively specify the dependencies this type provides.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ProvidesAttribute : Attribute
{
  /// <summary>
  ///   Default constructor.
  /// </summary>
  /// <param name="name">The provided dependency name.</param>
  public ProvidesAttribute(string name) => Name = name;

  /// <summary>
  ///   The name of the provided dependency.
  /// </summary>
  public string Name { get; }
}