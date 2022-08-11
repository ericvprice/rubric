namespace Rubric.Scripting;

public class AsyncRulesetModel<T>
{

  public List<string> Usings { get; set; }

  public List<string> References { get; set; }

  public RuleModel[] Rules { get; set; }

  public string BasePath { get; set; }

}

public class AsyncRulesetModel<T, U> : AsyncRulesetModel<T>
{
  public RuleModel[] PreRules { get; set; }

  public RuleModel[] PostRules { get; set; }

}
