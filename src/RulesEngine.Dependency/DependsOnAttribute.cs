namespace RulesEngine.Dependency
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class DependsOnAttribute : Attribute
  {
    public DependsOnAttribute(string name) => Name = name;

    public DependsOnAttribute(Type type) => Name = type.FullName ?? throw new ArgumentNullException(nameof(type));

    public string Name { get; }
  }
}