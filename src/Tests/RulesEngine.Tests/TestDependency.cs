using System.Collections.Generic;
using RulesEngine.Dependency;

namespace RulesEngine.Tests
{
    public class TestDependency : IDependency
    {
        public IEnumerable<string> Dependencies { get; set; } = new string[0];

        public IEnumerable<string> Provides { get; set; } = new string[0];

        public string Name { get; set; }
    }
}