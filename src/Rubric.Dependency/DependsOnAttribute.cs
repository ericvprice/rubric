namespace Rubric.Dependency;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
  public DependsOnAttribute(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name cannot be empty", nameof(name));
    Name = name;
  }

  public DependsOnAttribute(Type type) => Name = type.FullName ?? throw new ArgumentNullException(nameof(type));

  public string Name { get; }
}