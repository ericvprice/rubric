using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rubric.Extensions;

public static class AssemblyHelper
{
  internal static IEnumerable<Type> GetTypes<T>(this Assembly that, IEnumerable<string> includes = null, IEnumerable<string> excludes = null)
  {
    if (that is null) throw new ArgumentNullException(nameof(that));
    var types = that.GetTypes()
                    .Where(t => typeof(T).IsAssignableFrom(t));
    if (includes?.Any() ?? false)
      types = types.Where(t => includes.Any(i => i == t.Name || i == t.FullName));
    if (excludes?.Any() ?? false)
      types = types.Where(t => !excludes.Any(i => i == t.Name || i == t.FullName));
    return types;
  }
}
