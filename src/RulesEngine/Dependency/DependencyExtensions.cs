using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;

namespace RulesEngine.Dependency
{

    /// <summary>
    ///     Determine whether this object has any other dependencies.
    /// </summary>
    public static class DependencyExtensions
    {

        /// <summary>
        ///     Given a list of objects marked with dependency attributes, organize into list of lists, where each successive list
        ///     has mutually independent objects that are dependent on the objects in the previous lists.
        /// </summary>
        /// <typeparam name="T">The generic object type.</typeparam>
        /// <exception cref="ArgumentNullException">dependencies is null</exception>
        public static IEnumerable<IEnumerable<T>> ResolveDependencies<T>(this IEnumerable<T> dependencies) where T : class, IDependency
        {
            //Preconditions
            if (dependencies == null) throw new ArgumentNullException();
            dependencies = dependencies.ToArray();
            if (!dependencies.Any()) yield break;
            var depMap = dependencies.ToDictionary(d => d, d => d.Dependencies.ToArray());
            var resolvedDeps = new List<string>();
            var unresolved = dependencies.ToArray();

            while (unresolved.Any())
            {
                var (newResolved, newUnresolved) = unresolved.Partition(d => depMap[d].All(a => resolvedDeps.Contains(a)));
                newResolved = newResolved.ToArray();
                newUnresolved = newUnresolved.ToArray();
                if (!newResolved.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var r in unresolved)
                    {
                        var notFound = depMap[r].Where(dep => !resolvedDeps.Contains(dep)).ToArray();
                        var (circular, nonCircular) = notFound.Partition(n => unresolved.Any(u => u.Provides.Contains(n)));
                        circular = circular.ToArray();
                        nonCircular = nonCircular.ToArray();
                        if (nonCircular.Any())
                            sb.AppendLine($"Rule {typeof(T).FullName} has unsatisfied dependencies {string.Join(", ", nonCircular.Select(t => t))}.");
                        else if (circular.Any())
                            sb.AppendLine($"Rule {typeof(T).FullName} has circular dependencies {string.Join(", ", circular.Select(t => t))}.");
                    }
                    throw new DependencyException(sb.ToString());
                }
                resolvedDeps.AddRange(newResolved.Select(_ => _.Provides).SelectMany(_ => _));
                unresolved = newUnresolved.ToArray();
                yield return newResolved.ToArray();
            }
        }
    }
}