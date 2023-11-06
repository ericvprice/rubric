using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Engines;
using Rubric.Rules;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
internal class DefaultRuleEngine<T> : IRuleEngine<T> where T : class
{
  private readonly IRuleEngine<T> _instance;

  /// <summary>
  ///   DI constructor.
  /// </summary>
  /// <param name="builder">The builder.</param>
  /// <param name="rules">The rules.</param>
  public DefaultRuleEngine(IEngineBuilder<T> builder, IEnumerable<IRule<T>> rules)
    => _instance = builder.WithRules(rules).Build();

  /// <inheritdoc />
  public IEnumerable<IRule<T>> Rules => _instance.Rules;

  /// <inheritdoc />
  public ILogger Logger => _instance.Logger;

  /// <inheritdoc />
  public bool IsAsync => _instance.IsAsync;

  /// <inheritdoc />
  public Type InputType => _instance.InputType;

  /// <inheritdoc />
  public Type OutputType => _instance.OutputType;

  /// <inheritdoc />
  public IExceptionHandler ExceptionHandler => _instance.ExceptionHandler;

  /// <inheritdoc />
  public void Apply(T input, IEngineContext context = null)
    => _instance.Apply(input, context);

  /// <inheritdoc />
  public void Apply(IEnumerable<T> inputs, IEngineContext context = null)
    => _instance.Apply(inputs, context);
}