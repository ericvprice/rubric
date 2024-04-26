using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions.Serialization;

/// <summary>
///   Configuration model that specifies a rule.
/// </summary>
#pragma warning disable IDE0079 // Remove unnecessary suppression
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
public class RuleModel
{

  /// <summary>
  ///   A collection of dependencies provided by this rule (in addition to it's name).
  /// </summary>
  /// <returns>A collection of dependency names.</returns>
  public IEnumerable<string> Provides { get; set; } = [];

  /// <summary>
  ///   A collection of dependencies required by this rule (in addition to it's name).
  /// </summary>
  /// <returns>A collection of dependency names.</returns>
  public IEnumerable<string> DependsOn { get; set; } = [];

  /// <summary>
  ///   The predicate caching behavior for this rule.
  /// </summary>
  /// <value></value>
  public PredicateCachingJson PredicateCaching { get; set; }
    = new() { Behavior = CacheBehavior.None, Key = string.Empty };

  /// <summary>
  ///   Relative filepath of script to parse.
  /// </summary>
  /// <returns>A relative filepath.</returns>
  public string Script { get; set; }

  /// <summary>
  ///   The name of this rule.
  /// </summary>
  /// <value></value>
  public string Name { get; set; } = "";

}