using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace Rubric.Rules.Scripted;

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

  internal static ScriptOptions GetDefaultOptions<TIn, TOut>()
    => GetDefaultOptions<TIn>().WithReferences(typeof(TOut).Assembly);

  internal static string FilterScript(this string script)
  {
    using var sr = new StringReader(script);
    var output = new StringBuilder();
    while (sr.ReadLine() is { } line)
      if (!line.Trim().StartsWith("#r"))
        output.Append(line);
    return output.ToString();
  }
}