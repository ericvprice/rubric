namespace Rubric.Extensions.Serialization;

/// <summary>
///   Configuration model that specifies a rule.
/// </summary>
public class RuleModel
{
  /// <summary>
  ///   A collection of dependencies provided by this rule (in addition to it's name).
  /// </summary>
  /// <returns>A collection of dependency names.</returns>
  public string[] Provides { get; set; } = Array.Empty<string>();

  /// <summary>
  ///   A collection of dependencies required by this rule (in addition to it's name).
  /// </summary>
  /// <returns>A collection of dependency names.</returns>
  public string[] DependsOn { get; set; } = Array.Empty<string>();

  /// <summary>
  ///   Relative filepath of script to parse.
  /// </summary>
  /// <returns>A relative filepath.</returns>
  public string Script { get; set; } = null;

  /// <summary>
  ///   The name of this rule.
  /// </summary>
  /// <value></value>
  public string Name { get; set; } = "";

}