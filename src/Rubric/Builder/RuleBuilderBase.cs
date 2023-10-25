namespace Rubric.Builder;

internal class RuleBuilderBase
{
  internal RuleBuilderBase(string name)
  {
    Name = string.IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
    Provides.Add(Name);
  }

  internal List<string> Dependencies { get; } = new();

  internal List<string> Provides { get; } = new();

  internal PredicateCaching Caching { get; set; }

  internal string Name { get; }

  internal void AddDependency(string dep)
  {
    if (string.IsNullOrEmpty(dep)) throw new ArgumentException(nameof(dep));
    Dependencies.Add(dep);
  }

  internal void AddDependency(Type dep)
    => Dependencies.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));

  internal void AddProvides(string provides)
  {
    if (string.IsNullOrWhiteSpace(provides)) throw new ArgumentException(null, nameof(provides));
    Provides.Add(provides);
  }
}