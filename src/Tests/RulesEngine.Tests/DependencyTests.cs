using Xunit;
using RulesEngine.Dependency;
using System.Linq;

namespace RulesEngine.Tests
{

    public class DependencyTests
    {
        [Fact]
        public void Simple()
        {
            var dep1 = new TestDependency { Provides = new[] { "test" } };
            var dep2 = new TestDependency { Dependencies = new[] { "test" } };
            var depResult = new[] { dep1, dep2 }.ResolveDependencies().Select(e => e.ToArray()).ToArray();
            Assert.Equal(2, depResult.Length);
            Assert.Single(depResult[0]);
            Assert.Single(depResult[1]);
            Assert.Contains(dep1, depResult[0]);
            Assert.Contains(dep2, depResult[1]);

        }

        [Fact]
        public void Independent()
        {
            var dep1 = new TestDependency { Provides = new[] { "test" } };
            var dep2 = new TestDependency { Dependencies = new[] { "test" } };
            var dep3 = new TestDependency { Dependencies = new[] { "test" } };
            var depResult = new[] { dep1, dep2, dep3 }.ResolveDependencies().Select(e => e.ToArray()).ToArray();
            Assert.Equal(2, depResult.Length);
            Assert.Single(depResult[0]);
            Assert.Equal(2, depResult[1].Length);
            Assert.Contains(dep1, depResult[0]);
            Assert.Contains(dep2, depResult[1]);
            Assert.Contains(dep3, depResult[1]);

        }

        [Fact]
        public void DependentChain()
        {
            var dep1 = new TestDependency { Provides = new[] { "test" } };
            var dep2 = new TestDependency { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            var dep3 = new TestDependency { Dependencies = new[] { "test2" } };
            var depResult = new[] { dep1, dep2, dep3 }.ResolveDependencies().Select(e => e.ToArray()).ToArray();
            Assert.Equal(3, depResult.Length);
            Assert.Single(depResult[0]);
            Assert.Single(depResult[1]);
            Assert.Single(depResult[2]);
            Assert.Contains(dep1, depResult[0]);
            Assert.Contains(dep2, depResult[1]);
            Assert.Contains(dep3, depResult[2]);
        }

        [Fact]
        public void Cycle2()
        {
            var dep1 = new TestDependency { Provides = new[] { "test" }, Dependencies = new[] { "test2" } };
            var dep2 = new TestDependency { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            Assert.Throws<DependencyException>(() => new[] { dep1, dep2 }.ResolveDependencies().Select(e => e.ToArray()).ToArray());
        }

        [Fact]
        public void Cycle3()
        {
            var dep1 = new TestDependency { Provides = new[] { "test" }, Dependencies = new[] { "test2" } };
            var dep2 = new TestDependency { Provides = new[] { "test2" }, Dependencies = new[] { "test3" } };
            var dep3 = new TestDependency { Provides = new[] { "test3" }, Dependencies = new[] { "test" } };
            Assert.Throws<DependencyException>(() => new[] { dep1, dep2, dep3 }.ResolveDependencies().Select(e => e.ToArray()).ToArray());
        }

        [Fact]
        public void DeepCycle3()
        {
            var dep = new TestDependency { Provides = new[] { "test" } };
            var dep0 = new TestDependency { Provides = new[] { "test0" }, Dependencies = new[] { "test" } };
            var dep1 = new TestDependency { Provides = new[] { "test1" }, Dependencies = new[] { "test2", "test0" } };
            var dep2 = new TestDependency { Provides = new[] { "test2" }, Dependencies = new[] { "test3" } };
            var dep3 = new TestDependency { Provides = new[] { "test3" }, Dependencies = new[] { "test1" } };
            Assert.Throws<DependencyException>(() => new[] { dep, dep0, dep1, dep2, dep3 }.ResolveDependencies().Select(e => e.ToArray()).ToArray());
        }

        [Fact]
        public void Unresolved()
        {
            var dep1 = new TestDependency { Provides = new[] { "test" } };
            var dep2 = new TestDependency { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            var dep3 = new TestDependency { Dependencies = new[] { "test2", "test3" } };
            Assert.Throws<DependencyException>(() => new[] { dep1, dep2, dep3 }.ResolveDependencies().Select(e => e.ToArray()).ToArray());
        }

    }
}