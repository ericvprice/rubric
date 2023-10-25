using Rubric.Rules.Async;

namespace Rubric.Engines.Async;

/// <summary>
///   A rule engine that processes inputs and accumulates the results into an output type.
/// </summary>
/// <typeparam name="TIn">The input type</typeparam>
/// <typeparam name="TOut">The output type</typeparam>
public interface IRuleEngine<in TIn, in TOut> : IRuleEngine
    where TIn : class
    where TOut : class
{
    /// <summary>
    ///     The preprocessing rules for this engine.
    /// </summary>
    IEnumerable<IRule<TIn>> PreRules { get; }

    /// <summary>
    ///     The rules for this engine.
    /// </summary>
    IEnumerable<IRule<TIn, TOut>> Rules { get; }

    /// <summary>
    ///     The postprocessing rules for this engine.
    /// </summary>
    IEnumerable<IRule<TOut>> PostRules { get; }

    /// <summary>
    ///     Apply the given input to the output object.
    /// </summary>
    /// <param name="input">The input object.</param>
    /// <param name="output">The output object.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="token">An optional cancellation token.</param>
    Task ApplyAsync(TIn input, TOut output, IEngineContext context = null, CancellationToken token = default);

    /// <summary>
    ///     Serially apply the given inputs to the output object.
    /// </summary>
    /// <param name="inputs">The input objects.</param>
    /// <param name="output">The output object.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="token">An optional cancellation token.</param>
    Task ApplyAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default);

    /// <summary>
    ///     Apply the given inputs to the output object in parallel.
    /// </summary>
    /// <param name="inputs">The input objects.</param>
    /// <param name="output">The output object.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="token">An optional cancellation token.</param>
    Task ApplyParallelAsync(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null, CancellationToken token = default);

    /// <summary>
    ///     Stream inputs and apply them serially to the given output object.
    /// </summary>
    /// <param name="inputStream">The input objects.</param>
    /// <param name="output">The output object.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="token">An optional cancellation token.</param>
    Task ApplyAsync(IAsyncEnumerable<TIn> inputStream, TOut output, IEngineContext context = null, CancellationToken token = default);
}