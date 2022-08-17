namespace Rubric.Extensions;

public static class AssemblyHelper
{
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
