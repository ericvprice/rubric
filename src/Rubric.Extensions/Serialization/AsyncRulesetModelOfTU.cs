using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions.Serialization;

#pragma warning disable IDE0079 // Remove unnecessary suppression
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
public class AsyncRulesetModel<TIn, TOut> : AsyncRulesetModel<TIn>
{

  public Type OutputType => typeof(TOut);
  
  /// <summary>
  ///   A mapping of names => preprocessing rule definitions.
  /// </summary>
  /// <value>The configured rule information.</value>
  public Dictionary<string, RuleModel> PreRules { get; set; } = new();

  /// <summary>
  ///   A mapping of names => postprocessing rule definitions.
  /// </summary>
  /// <value>The configured rule information.</value>
  public Dictionary<string, RuleModel> PostRules { get; set; } = new();

}
