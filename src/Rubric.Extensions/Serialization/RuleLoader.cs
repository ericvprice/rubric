using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rules.Async;
using Rubric.Rules.Scripted;
using System.IO;

namespace Rubric.Extensions.Serialization;

internal static class RuleLoader
{

  internal static IAsyncRule<X> LoadFromModel<X>(RuleModel model, string basePath, ScriptOptions options)
   => new ScriptedRule<X>
      (
        model.Name,
        File.ReadAllText(Path.Combine(basePath, model.Script)),
        options,
        model.DependsOn,
        model.Provides
      );

  internal static IAsyncRule<X, Y> LoadFromModel<X, Y>(RuleModel model, string basePath, ScriptOptions options)
    => new ScriptedRule<X, Y>(
          model.Name,
          File.ReadAllText(Path.Combine(basePath, model.Script)),
          options,
          model.DependsOn,
          model.Provides);

}