namespace Rubric.Extensions.Serialization;

public class RuleModel
{
  public string[] Provides { get; set; } = Array.Empty<string>();

  public string[] DependsOn { get; set; } = Array.Empty<string>();

  public string Script { get; set; } = null;

  public string Name { get; set; } = "";
}