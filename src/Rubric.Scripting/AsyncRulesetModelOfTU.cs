namespace Rubric.Scripting;

public class AsyncRulesetModel<T, U> : AsyncRulesetModel<T>
{
  public Dictionary<string, RuleModel> PreRules { get; set; } = new();

  public Dictionary<string, RuleModel> PostRules { get; set; } = new();

}
