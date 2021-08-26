using RulesEngine.Dependency;

namespace RulesEngine.Tests
{
    public class TestDependency : IDependency
    {

        public TestDependency(string name) => Name = name;

        public IEnumerable<string> Dependencies { get; set; } = new string[0];

        public IEnumerable<string> Provides { get; set; } = new string[0];

        public string Name { get; set; }
    }
}