using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rules.Async;
using static Rubric.Extensions.Serialization.RuleLoader;

namespace Rubric.Extensions.Serialization;

internal class JsonRuleSet<T, U> : AsyncRuleset<T, U>
{
  internal JsonRuleSet(AsyncRulesetModel<T, U> model, ScriptOptions options) : base()
  {
    if (model == null)
      throw new ArgumentNullException($"'{nameof(model)}' cannot be null.", nameof(model));
    AddAsyncPreRules(model.PreRules
                          .Select(r =>
                          {
                            r.Value.Name = r.Key;
                            return LoadFromModel<T>(r.Value, model.BasePath, options);
                          }));
    AddAsyncRules(model.Rules
                       .Select(r =>
                        {
                          r.Value.Name = r.Key;
                          return LoadFromModel<T, U>(r.Value, model.BasePath, options);
                        }));
    AddAsyncPostRules(model.PostRules
                           .Select(r =>
                           {
                             r.Value.Name = r.Key;
                             return LoadFromModel<U>(r.Value, model.BasePath, options);
                           }));
  }
}