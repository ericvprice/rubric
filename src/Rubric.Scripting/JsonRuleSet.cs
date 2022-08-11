using static Rubric.Scripting.RuleLoader;

namespace Rubric.Scripting;

public class JsonRuleSet<T> : AsyncRuleset<T>
{
  public JsonRuleSet(AsyncRulesetModel<T> model) : base()
  {
    if (model == null)
      throw new ArgumentNullException($"'{nameof(model)}' cannot be null.", nameof(model));
    AddAsyncRules(model.Rules.Select(r => LoadFromModel<T>(r, model.BasePath)));
  }
}

public class JsonRuleSet<T, U> : AsyncRuleset<T, U>
{
  public JsonRuleSet(AsyncRulesetModel<T, U> model) : base()
  {
    if (model == null)
      throw new ArgumentNullException($"'{nameof(model)}' cannot be null.", nameof(model));
    AddAsyncPreRules(model.PreRules.Select(r => LoadFromModel<T>(r, model.BasePath)));
    AddAsyncRules(model.Rules.Select(r => LoadFromModel<T, U>(r, model.BasePath)));
    AddAsyncPostRules(model.PostRules.Select(r => LoadFromModel<U>(r, model.BasePath)));
  }
}