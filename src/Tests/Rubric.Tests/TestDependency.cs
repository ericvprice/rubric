using Rubric.Dependency;

namespace Rubric.Tests;

public class TestDependency : IDependency
{

  public TestDependency(string name) => Name = name;

  public IEnumerable<string> Dependencies { get; init; } = Array.Empty<string>();

  public IEnumerable<string> Provides { get; init; } = Array.Empty<string>();

  public string Name { get; }
}