namespace Rubric.Scripting;

public class AsyncRulesetModel<T>
{

  public string BasePath { get; set; }

  public Dictionary<string, RuleModel> Rules { get; set; }

}
