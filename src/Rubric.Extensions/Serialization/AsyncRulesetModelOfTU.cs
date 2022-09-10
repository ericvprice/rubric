namespace Rubric.Extensions.Serialization;

public class AsyncRulesetModel<TIn, TOut> : AsyncRulesetModel<TIn>
{
  public Dictionary<string, RuleModel> PreRules { get; set; } = new();

  public Dictionary<string, RuleModel> PostRules { get; set; } = new();

}
