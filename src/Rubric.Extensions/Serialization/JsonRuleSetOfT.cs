using Rubric.Rules.Async;
using static Rubric.Extensions.Serialization.RuleLoader;

namespace Rubric.Extensions.Serialization;

internal class JsonRuleSet<T> : AsyncRuleset<T>
{
  public JsonRuleSet(AsyncRulesetModel<T> model) : base()
  {
    if (model == null)
      throw new ArgumentNullException($"'{nameof(model)}' cannot be null.", nameof(model));
    AddAsyncRules(model.Rules
                       .Select(r =>
                          {
                            r.Value.Name = r.Key;
                            return LoadFromModel<T>(r.Value, model.BasePath);
                          }));
  }
}
