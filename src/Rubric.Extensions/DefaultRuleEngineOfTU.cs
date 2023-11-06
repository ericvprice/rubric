using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Engines;
using Rubric.Rules;
using System.Diagnostics.CodeAnalysis;

namespace Rubric.Extensions;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
internal class DefaultRuleEngine<TIn, TOut> : IRuleEngine<TIn, TOut> where TIn : class where TOut : class
{
  private readonly IRuleEngine<TIn, TOut> _instance;

  /// <summary>
  ///   DI constructor.
  /// </summary>
  /// <param name="builder">The rules builder.</param>
  /// <param name="preRules">The preprocessing rules.</param>
  /// <param name="rules">The rules.</param>
  /// <param name="postRules">The post-processing rules.</param>
  public DefaultRuleEngine(
    IEngineBuilder<TIn, TOut> builder,
    IEnumerable<IRule<TIn>> preRules,
    IEnumerable<IRule<TIn, TOut>> rules,
    IEnumerable<IRule<TOut>> postRules
  ) => _instance = builder.WithPreRules(preRules)
                          .WithRules(rules)
                          .WithPostRules(postRules)
                          .Build();
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
  public IEnumerable<IRule<TIn>> PreRules => _instance.PreRules;

  /// <inheritdoc />
  public IEnumerable<IRule<TOut>> PostRules => _instance.PostRules;

  /// <inheritdoc />
  public IEnumerable<IRule<TIn, TOut>> Rules => _instance.Rules;

  /// <inheritdoc />
  public void Apply(TIn input, TOut output, IEngineContext context = null)
    => _instance.Apply(input, output, context);

  public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
    => _instance.Apply(inputs, output, context);
}