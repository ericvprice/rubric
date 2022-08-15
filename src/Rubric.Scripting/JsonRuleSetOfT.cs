using static Rubric.Scripting.RuleLoader;

namespace Rubric.Scripting;

public class JsonRuleSet<T> : AsyncRuleset<T>
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
