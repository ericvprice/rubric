using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rules.Async;
using Rubric.Rules.Scripted;
using System.IO;

namespace Rubric.Extensions.Serialization;

/// <summary>
///   Helper class to turn a configured rule into a scripted rule.
/// </summary>
internal static class RuleLoader
{

  internal static IRule<T> LoadFromModel<T>(RuleModel model, string basePath, ScriptOptions options)
   => new ScriptedRule<T>
      (
        model.Name,
        File.ReadAllText(Path.Combine(basePath, model.Script)),
        options,
        model.DependsOn,
        model.Provides,
        new (model.PredicateCaching.Behavior,
             model.PredicateCaching.Key ?? model.Name)
      );

  internal static IRule<TIn, TOut> LoadFromModel<TIn, TOut>(RuleModel model, string basePath, ScriptOptions options)
    => new ScriptedRule<TIn, TOut>(
          model.Name,
          File.ReadAllText(Path.Combine(basePath, model.Script)),
          options,
          model.DependsOn,
          model.Provides,
          new (model.PredicateCaching.Behavior,
               model.PredicateCaching.Key ?? model.Name));

}