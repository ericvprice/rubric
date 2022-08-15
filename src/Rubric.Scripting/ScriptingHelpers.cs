using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace Rubric.Scripting;

internal static class ScriptingHelpers
{
  internal static ScriptOptions GetDefaultOptions<T>()
    => ScriptOptions.Default
            .WithReferences(typeof(ScriptedRuleContext<T>).Assembly,
                            typeof(EngineContext).Assembly,
                            typeof(ILogger).Assembly,
                            typeof(T).Assembly)
            .AddImports("Rubric",
                        "System.Threading",
                        "System.Threading.Tasks");

  internal static ScriptOptions GetDefaultOptions<T, U>()
    => GetDefaultOptions<T>().WithReferences(typeof(U).Assembly);

  internal static string FilterScript(this string script)
  {
    using var sr = new StringReader(script);
    string line;
    var output = new StringBuilder();
    while ((line = sr.ReadLine()) != null)
    {
      if (!line.Trim().StartsWith("#r"))
        output.Append(line);
    }
    return output.ToString();
  }
}