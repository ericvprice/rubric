using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using Rubric.Rules.Async;

namespace Rubric.Scripting
{
  public static class RuleLoader
  {

    public static IEnumerable<IAsyncRule<X>> LoadJsonFromGlob<X>(string glob)
    {
      var matcher = new Matcher();
      matcher.AddIncludePatterns(new[] { glob });
      var results = matcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
      foreach (var file in results)
      {
        var model = JsonSerializer.Deserialize<RuleModel>(File.ReadAllText(file));
        yield return new ScriptedRule<X>(
            model.Name,
            string.IsNullOrWhiteSpace(model.DoesApplyScript)
                ? string.IsNullOrWhiteSpace(model.DoesApply)
                    ? "return true"
                    : model.DoesApply
                : File.ReadAllText(model.DoesApplyScript),
            string.IsNullOrWhiteSpace(model.ApplyScript)
                ? string.IsNullOrWhiteSpace(model.Apply)
                    ? "return true"
                    : model.DoesApply
                : File.ReadAllText(model.ApplyScript),
            model.DependsOn,
            model.Provides);
      }
    }

    public static IEnumerable<IAsyncRule<X, Y>> LoadJsonFromGlob<X, Y>(string glob)
    {
      var matcher = new Matcher();
      matcher.AddIncludePatterns(new[] { glob });
      var results = matcher.GetResultsInFullPath(Directory.GetCurrentDirectory());
      foreach (var file in results)
      {
        var model = JsonSerializer.Deserialize<RuleModel>(File.ReadAllText(file));
        yield return new ScriptedRule<X, Y>(
            model.Name,
            string.IsNullOrWhiteSpace(model.DoesApplyScript)
                ? string.IsNullOrWhiteSpace(model.DoesApply)
                    ? "return true"
                    : model.DoesApply
                : File.ReadAllText(model.DoesApplyScript),
            string.IsNullOrWhiteSpace(model.ApplyScript)
                ? string.IsNullOrWhiteSpace(model.Apply)
                    ? "return true"
                    : model.DoesApply
                : File.ReadAllText(model.ApplyScript),
            model.DependsOn,
            model.Provides);
      }
    }

  }
}