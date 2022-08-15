using static Rubric.Scripting.RuleLoader;

namespace Rubric.Scripting;

public class JsonRuleSet<T, U> : AsyncRuleset<T, U>
{
  public JsonRuleSet(AsyncRulesetModel<T, U> model) : base()
  {
    if (model == null)
      throw new ArgumentNullException($"'{nameof(model)}' cannot be null.", nameof(model));
    AddAsyncPreRules(model.PreRules
                          .Select(r =>
                          {
                            r.Value.Name = r.Key;
                            return LoadFromModel<T>(r.Value, model.BasePath);
                          }));
    AddAsyncRules(model.Rules
                       .Select(r =>
                        {
                          r.Value.Name = r.Key;
                          return LoadFromModel<T, U>(r.Value, model.BasePath);
                        }));
    AddAsyncPostRules(model.PostRules
                           .Select(r =>
                           {
                             r.Value.Name = r.Key;
                             return LoadFromModel<U>(r.Value, model.BasePath);
                           }));
  }
}