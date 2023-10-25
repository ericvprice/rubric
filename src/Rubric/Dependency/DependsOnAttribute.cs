namespace Rubric.Dependency;

/// <summary>
///   Declarative attribute for specifying dependencies.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute : Attribute
{
  /// <summary>
  ///  The type specified, if any.
  /// </summary>
  public Type Type { get; }

  /// <summary>
  ///   Construct a named dependency.
  /// </summary>
  /// <param name="name">The name.</param>
  /// <exception cref="ArgumentException">Name is null or empty.</exception>
  public DependsOnAttribute(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name cannot be empty", nameof(name));
    Name = name;
  }

  /// <summary>
  ///   Construct a dependency on a type.
  /// </summary>
  /// <param name="type">A dependency on a type.</param>
  public DependsOnAttribute(Type type)
  {
    Type = type;
    Name = type?.FullName ?? throw new ArgumentNullException(nameof(type));
  }

  /// <summary>
  ///   The name of this dependency.
  /// </summary>
  public string Name { get; }

}