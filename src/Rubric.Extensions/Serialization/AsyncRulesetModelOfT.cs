using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions.Serialization;

/// <summary>
///   Represents an asynchronous ruleset configuration.
/// </summary>
/// <typeparam name="T">The rule type parameters.</typeparam>
#pragma warning disable IDE0079
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
#pragma warning restore IDE0079
public class AsyncRulesetModel<T>
{

  /// <summary>
  ///   The ruleset output type.
  /// </summary>
  public Type InputType => typeof(T);

  /// <summary>
  ///   The base path of the loaded rules.
  /// </summary>
  /// <value>A directory path.</value>
  public string BasePath { get; set; }

  /// <summary>
  ///   A mapping of names => rule definitions.
  /// </summary>
  /// <value>The configured rule information.</value>
  public Dictionary<string, RuleModel> Rules { get; } = [];

}
