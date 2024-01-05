using Microsoft.Extensions.Logging;
using Rubric.Rules.Async;

namespace Rubric.Engines.Async.Implementation;

internal class ChainedEngine<TIn, TInt, TOut>(IRuleEngine<TIn, TInt> first, IRuleEngine<TInt, TOut> second,
                                              Func<TInt> intFactory)
  : IRuleEngine<TIn, TOut>
  where TIn : class
  where TInt : class
  where TOut : class
{

  private readonly Func<TInt> _intFactory = intFactory ?? throw new ArgumentNullException(nameof(intFactory));
  
  public IRuleEngine<TIn, TInt> First { get; } = first ?? throw new ArgumentNullException(nameof(first));

  public IRuleEngine<TInt, TOut> Second { get; } = second ?? throw new ArgumentNullException(nameof(second));


  /// <inheritdoc />
  public ILogger Logger => throw new NotImplementedException();

  /// <inheritdoc />
  public bool IsAsync => true;

  /// <inheritdoc />
  public Type InputType => typeof(TIn);

  /// <inheritdoc />
  public Type OutputType => typeof(TOut);

  /// <inheritdoc />
  public IExceptionHandler ExceptionHandler => throw new NotImplementedException();

  /// <inheritdoc />
  public IEnumerable<IRule<TIn>> PreRules => First.PreRules;

  /// <inheritdoc />
  public IEnumerable<IRule<TIn, TOut>> Rules => throw new NotImplementedException();

  /// <inheritdoc />
  public IEnumerable<IRule<TOut>> PostRules => Second.PostRules;

  public bool IsParallel => First.IsParallel || Second.IsParallel;

  /// <inheritdoc />
  public async Task ApplyAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    var intermediate = _intFactory();
    context ??= new EngineContext();
    await First.ApplyAsync(input, intermediate, context, token).ConfigureAwait(false);
    await Second.ApplyAsync(intermediate, output, context, token).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    var intermediate = _intFactory();
    context ??= new EngineContext();
    await First.ApplyAsync(inputs, intermediate, context, token).ConfigureAwait(false);
    await Second.ApplyAsync(intermediate, output, context, token).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task ApplyAsync(IAsyncEnumerable<TIn> inputStream, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    var intermediate = _intFactory();
    context ??= new EngineContext();
    await First.ApplyAsync(inputStream, intermediate, context, token).ConfigureAwait(false);
    await Second.ApplyAsync(intermediate, output, context, token).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task ApplyParallelAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default)
  {
    var intermediate = _intFactory();
    context ??= new EngineContext();
    await First.ApplyParallelAsync(inputs, intermediate, context, token).ConfigureAwait(false);
    await Second.ApplyAsync(intermediate, output, context, token).ConfigureAwait(false);
  }
}