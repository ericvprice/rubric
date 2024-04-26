using Microsoft.CodeAnalysis.Scripting;
using Rubric.Rulesets.Async;
using static Rubric.Extensions.Serialization.RuleLoader;

namespace Rubric.Extensions.Serialization;

internal class JsonRuleSet<T> : Ruleset<T>
{
  public JsonRuleSet(AsyncRulesetModel<T> model, ScriptOptions options = null)
  {
    if (model == null) throw new ArgumentNullException(nameof(model));
    AddAsyncRules(model.Rules
                       .Select(r =>
                          {
                            r.Value.Name = r.Key;
                            return LoadFromModel<T>(r.Value, model.BasePath, options);
                          }));
  }
}
