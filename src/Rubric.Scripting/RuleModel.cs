using System;

namespace Rubric.Scripting
{
  public class RuleModel
  {
    public string[] Provides { get; set; } = Array.Empty<string>();

    public string[] DependsOn { get; set; } = Array.Empty<string>();

    public string DoesApply { get; set; } = "return true";

    public string Apply { get; set; } = "return";

    public string DoesApplyScript { get; set; } = null;

    public string ApplyScript { get; set; } = null;

    public string Name { get; set; } = "";
  }
}