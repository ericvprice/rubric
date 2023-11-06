namespace Rubric.Extensions;

/// <summary>
///   Static container for extension methods.
/// </summary>
public static class AssemblyExtensions
{
  /// <summary>
  ///   Get all implementations of a given type in an assembly.
  /// </summary>
  /// <typeparam name="T">The type to scan for.</typeparam>
  /// <param name="that">The assembly to scan.</param>
  /// <param name="includes">A list of types to include.</param>
  /// <param name="excludes">A list of types to exclude.  Exclusions take priority of inclusions.</param>
  /// <returns>A list of types that implement/extend the given type.</returns>
  /// <exception cref="ArgumentNullException">The provided assembly is null.</exception>
  internal static IEnumerable<Type> GetTypes<T>(this Assembly that, IEnumerable<string> includes = null, IEnumerable<string> excludes = null)
  {
    if (that is null) throw new ArgumentNullException(nameof(that));
    var types = that.GetTypes()
                    .Where(t => typeof(T).IsAssignableFrom(t));
    includes = includes?.ToArray() ?? Array.Empty<string>();
    excludes = excludes?.ToArray() ?? Array.Empty<string>();
    if (includes.Any())
      types = types.Where(t => includes.Any(i => i == t.Name || i == t.FullName));
    if (excludes.Any())
      types = types.Where(t => !excludes.Any(i => i == t.Name || i == t.FullName));
    return types;
  }
}
