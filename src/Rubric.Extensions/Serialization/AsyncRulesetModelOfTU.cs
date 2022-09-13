namespace Rubric.Extensions.Serialization;

public class AsyncRulesetModel<TIn, TOut> : AsyncRulesetModel<TIn>
{

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
