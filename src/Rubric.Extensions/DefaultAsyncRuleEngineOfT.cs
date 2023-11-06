using Microsoft.Extensions.Logging;
using Rubric.Rules.Async;
using System.Diagnostics.CodeAnalysis;
using Rubric.Engines.Async;
using Rubric.Builder.Async;

namespace Rubric.Extensions;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
internal class DefaultAsyncRuleEngine<T> : IRuleEngine<T> where T : class
{
  private readonly IRuleEngine<T> _instance;

  /// <summary>
  ///   DI constructor taking an engine builder and a set of rules.
  /// </summary>
  /// <param name="builder">The engine builder.</param>
  /// <param name="rules">The rules.</param>
  public DefaultAsyncRuleEngine(IEngineBuilder<T> builder, IEnumerable<IRule<T>> rules)
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
  public bool IsParallel => _instance.IsParallel;

  /// <inheritdoc />
  public Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(input, context, token);

  /// <inheritdoc />
  public Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null, bool parallelizeInputs = false, CancellationToken token = default)
    => _instance.ApplyAsync(inputs, context, parallelizeInputs, token);
  
  /// <inheritdoc />
  public Task ApplyAsync(IAsyncEnumerable<T> inputStream, IEngineContext context = null, CancellationToken token = default)
    => _instance.ApplyAsync(inputStream, context, token);
}