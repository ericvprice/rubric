using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rulesets.Async;
using static Rubric.Extensions.Serialization.RuleLoader;

namespace Rubric.Extensions.Serialization;

internal class JsonRuleSet<TIn, TOut> : Ruleset<TIn, TOut>
{
  internal JsonRuleSet(AsyncRulesetModel<TIn, TOut> model, ScriptOptions options)
  {
    if (model == null)
      throw new ArgumentNullException($"'{nameof(model)}' cannot be null.", nameof(model));
    AddPreRules(model.PreRules
                          .Select(r =>
                          {
                            r.Value.Name = r.Key;
                            return LoadFromModel<TIn>(r.Value, model.BasePath, options);
                          }));
    AddRules(model.Rules
                       .Select(r =>
                        {
                          r.Value.Name = r.Key;
                          return LoadFromModel<TIn, TOut>(r.Value, model.BasePath, options);
                        }));
    AddPostRules(model.PostRules
                           .Select(r =>
                           {
                             r.Value.Name = r.Key;
                             return LoadFromModel<TOut>(r.Value, model.BasePath, options);
                           }));
  }
}