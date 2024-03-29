namespace Rubric.Dependency;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ProvidesAttribute : Attribute
{
  public ProvidesAttribute(string name) => Name = name;

  public string Name { get; }
}