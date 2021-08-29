namespace RulesEngine.Dependency
{
    /// <summary>
    ///     Determine whether this object has any other dependencies.
    /// </summary>
    public static class DependencyExtensions
    {
        public static IEnumerable<IEnumerable<T>> ResolveDependencies<T>(this IEnumerable<T> dependencies)
            where T : class, IDependency
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            var depList = dependencies.ToList();
            if (!depList.Any()) return Array.Empty<T[]>();

            //Setup local lists and dictionary lookups
            var resolvedObjects = new List<T>();
            var resolvedDependencies = new List<string>();
            var depMap = depList.ToDictionary(d => d, d => d.Dependencies.ToArray());
            var providesMap = depList.ToDictionary(d => d, d => d.Provides.ToArray());
            var providerMap = depList.SelectMany(d => d.Provides)
                                     .Distinct()
                                     .ToDictionary(
                                         p => p, p => depList.Where(d => d.Provides.Contains(p)).ToList());

            var toReturn = new List<List<T>>();

            void UpdateResolved(List<T> newlyResolvedObjects)
            {
                resolvedObjects.AddRange(newlyResolvedObjects);
                //Add dependencies that have all of their providers now resolved
                resolvedDependencies.AddRange(
                    newlyResolvedObjects.SelectMany(o => providesMap[o])
                                        .Distinct()
                                        .Where(p => providerMap[p].All(o => resolvedObjects.Contains(o)))
                );
                toReturn.Add(newlyResolvedObjects);
                foreach (var res in newlyResolvedObjects) depList.Remove(res);
            }

            CheckForMissing(depList, providerMap);

            //Find root dependencies
            var roots = depMap.Where(d => !d.Value.Any())
                              .Select(d => d.Key)
                              .ToList();
            if (!roots.Any()) throw new DependencyException("No roots found.");
            UpdateResolved(roots);


            while (depList.Any())
            {
                var newlyResolvedDependencies =
                    depList.Where(d => depMap[d].All(d1 => resolvedDependencies.Contains(d1)))
                           .ToList();
                //Uh oh, we hit a brick wall.
                //We know we have a closed set of dependencies from above, so
                //the only way this happens is if we have a circular dependency somewhere in the dependencies left.
                if (!newlyResolvedDependencies.Any())
                    throw new DependencyException("Circular dependencies found.")
                    {
                        Details = new List<string> { FindCycle(depList) }
                    };
                UpdateResolved(newlyResolvedDependencies);
            }

            return toReturn;
        }

        private static void CheckForMissing<T>(IReadOnlyCollection<T> depList, IReadOnlyDictionary<string, List<T>> providerMap) where T : class, IDependency
        {
            //Check that all dependencies have at least one provider.
            var depNotFound = depList.SelectMany(d => d.Dependencies)
                                     .Distinct()
                                     .Where(d => !providerMap.ContainsKey(d)).ToArray();
            if (depNotFound.Any())
            {
                var e = new DependencyException("Missing dependencies.");
                var errorList = new List<string>();
                foreach (var dep in depNotFound)
                {
                    var oList = depList.Where(d => d.Dependencies.Contains(dep)).Select(d => d.Name).ToArray();
                    errorList.Add($"{string.Join(", ", oList)} depend(s) on missing dependency {dep}.");
                }

                e.Details = errorList;
                throw e;
            }
        }

        private static string FindCycle<T>(IEnumerable<T> deps) where T : class, IDependency
        {
            var depList = deps.ToList();
            var paths = new List<List<T>>(depList.Select(d => new List<T> { d }));
            while (true)
            {
                var path = paths.First();
                paths.Remove(path);
                var newPaths = path.Last()
                                   .Dependencies
                                   .SelectMany(d => depList.Where(d1 => d1.Provides.Contains(d)))
                                   .Select(d => new List<T>(path) { d })
                                   .ToList();
                foreach (var newPath in newPaths)
                {
                    var index = newPath.IndexOf(path.Last());
                    if (index == path.Count - 1) continue;
                    var cycleList = newPath.GetRange(index, newPath.Count - 1).Select(d => d.Name);
                    return $"Dependency cycle {string.Join("->", cycleList)}";
                }

                paths.AddRange(newPaths);
            }
        }
    }
}