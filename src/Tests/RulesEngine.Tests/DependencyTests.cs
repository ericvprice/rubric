using System;
using System.Linq;
using RulesEngine.Dependency;
using Xunit;

namespace RulesEngine.Tests
{
    public class DependencyTests
    {
        [Fact]
        public void Cycle2()
        {
            //Added to avoid no root exception
            var dep = new TestDependency("dep");
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" }, Dependencies = new[] { "test2" } };
            var dep2 = new TestDependency("dep2") { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            var ex = Assert.Throws<DependencyException>(() => new[] { dep, dep1, dep2 }
                                                              .ResolveDependencies()
                                                              .Select(e => e.ToArray())
                                                              .ToArray());
            Assert.Equal("Circular dependencies found.", ex.Message);
            Assert.NotEmpty(ex.Details);
            var detail = ex.Details.First();
            Assert.NotEqual(-1, detail.IndexOf("dep1", StringComparison.Ordinal));
            Assert.NotEqual(-1, detail.IndexOf("dep2", StringComparison.Ordinal));
        }

        [Fact]
        public void Cycle3()
        {
            //Added to avoid no root exception
            var dep = new TestDependency("dep");
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" }, Dependencies = new[] { "test2" } };
            var dep2 = new TestDependency("dep2") { Provides = new[] { "test2" }, Dependencies = new[] { "test3" } };
            var dep3 = new TestDependency("dep3") { Provides = new[] { "test3" }, Dependencies = new[] { "test" } };
            var ex = Assert.Throws<DependencyException>(() => new[] { dep, dep1, dep2, dep3 }
                                                              .ResolveDependencies()
                                                              .Select(e => e.ToArray())
                                                              .ToArray());
            Assert.NotEmpty(ex.Details);
            var detail = ex.Details.First();
            Assert.NotEqual(-1, detail.IndexOf("dep1", StringComparison.Ordinal));
            Assert.NotEqual(-1, detail.IndexOf("dep2", StringComparison.Ordinal));
            Assert.NotEqual(-1, detail.IndexOf("dep3", StringComparison.Ordinal));
        }

        [Fact]
        public void DeepCycle3()
        {
            var dep = new TestDependency("dep1") { Provides = new[] { "test" } };
            var dep0 = new TestDependency("dep2") { Provides = new[] { "test0" }, Dependencies = new[] { "test" } };
            var dep1 = new TestDependency("dep3")
                { Provides = new[] { "test1" }, Dependencies = new[] { "test2", "test0" } };
            var dep2 = new TestDependency("dep4") { Provides = new[] { "test2" }, Dependencies = new[] { "test3" } };
            var dep3 = new TestDependency("dep5") { Provides = new[] { "test3" }, Dependencies = new[] { "test1" } };
            Assert.Throws<DependencyException>(() => new[] { dep, dep0, dep1, dep2, dep3 }
                                                     .ResolveDependencies().Select(e => e.ToArray()).ToArray());
        }

        [Fact]
        public void DependentChain()
        {
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" } };
            var dep2 = new TestDependency("dep2") { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            var dep3 = new TestDependency("dep3") { Dependencies = new[] { "test2" } };
            var depResult = new[] { dep1, dep2, dep3 }
                            .ResolveDependencies()
                            .Select(e => e.ToArray())
                            .ToArray();
            Assert.Equal(3, depResult.Length);
            Assert.Single(depResult[0]);
            Assert.Single(depResult[1]);
            Assert.Single(depResult[2]);
            Assert.Contains(dep1, depResult[0]);
            Assert.Contains(dep2, depResult[1]);
            Assert.Contains(dep3, depResult[2]);
        }

        [Fact]
        public void Independent()
        {
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" } };
            var dep2 = new TestDependency("dep2") { Dependencies = new[] { "test" } };
            var dep3 = new TestDependency("dep3") { Dependencies = new[] { "test" } };
            var depResult = new[] { dep1, dep2, dep3 }
                            .ResolveDependencies()
                            .Select(e => e.ToArray())
                            .ToArray();
            Assert.Equal(2, depResult.Length);
            Assert.Single(depResult[0]);
            Assert.Equal(2, depResult[1].Length);
            Assert.Contains(dep1, depResult[0]);
            Assert.Contains(dep2, depResult[1]);
            Assert.Contains(dep3, depResult[1]);
        }

        [Fact]
        public void MultipleProviders()
        {
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test", "dep1" } };
            var dep2 = new TestDependency("dep2") { Provides = new[] { "test" }, Dependencies = new[] { "dep1" } };
            var dep3 = new TestDependency("dep3") { Dependencies = new[] { "test" } };
            var deplist = new[] { dep1, dep2, dep3 }
                          .ResolveDependencies()
                          .Select(e => e.ToArray())
                          .ToArray();
            Assert.Equal(3, deplist.Length);
            Assert.Single(deplist[0]);
            Assert.Contains(dep1, deplist[0]);
            Assert.Single(deplist[1]);
            Assert.Contains(dep2, deplist[1]);
            Assert.Single(deplist[2]);
            Assert.Contains(dep3, deplist[2]);
        }

        [Fact]
        public void NoRoots()
        {
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" }, Dependencies = new[] { "test2" } };
            var dep2 = new TestDependency("dep2") { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            var ex = Assert.Throws<DependencyException>(
                () => new[] { dep1, dep2 }
                      .ResolveDependencies()
                      .Select(e => e.ToArray())
                      .ToArray());
            Assert.Equal("No roots found.", ex.Message);
        }

        [Fact]
        public void Simple()
        {
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" } };
            var dep2 = new TestDependency("dep2") { Dependencies = new[] { "test" } };
            var depResult = new[] { dep1, dep2 }
                            .ResolveDependencies()
                            .Select(e => e.ToArray())
                            .ToArray();
            Assert.Equal(2, depResult.Length);
            Assert.Single(depResult[0]);
            Assert.Single(depResult[1]);
            Assert.Contains(dep1, depResult[0]);
            Assert.Contains(dep2, depResult[1]);
        }

        [Fact]
        public void Unresolved()
        {
            var dep1 = new TestDependency("dep1") { Provides = new[] { "test" } };
            var dep2 = new TestDependency("dep2") { Provides = new[] { "test2" }, Dependencies = new[] { "test" } };
            var dep3 = new TestDependency("dep3") { Dependencies = new[] { "test2", "test3" } };
            var ex = Assert.Throws<DependencyException>(() => new[] { dep1, dep2, dep3 }
                                                              .ResolveDependencies()
                                                              .Select(e => e.ToArray())
                                                              .ToArray());
            Assert.Single(ex.Details);
            Assert.Equal("dep3 depend(s) on missing dependency test3.", ex.Details.First());
        }
    }
}