using Rubric.Dependency;

namespace Rubric.Tests
{
  public class TestDependency : IDependency
  {

    public TestDependency(string name) => Name = name;

    public IEnumerable<string> Dependencies { get; set; } = Array.Empty<string>();

    public IEnumerable<string> Provides { get; set; } = Array.Empty<string>();

    public string Name { get; set; }
  }
}