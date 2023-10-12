using Rubric.Rules.Async;

namespace Rubric.Engines.Async;

public interface IRuleEngine<in T> : IRuleEngine
    where T : class
{
    /// <summary>
    ///     The rules for this engine.
    /// </summary>
    IEnumerable<IRule<T>> Rules { get; }

    /// <summary>
    ///     Apply the given input to the output object.
    /// </summary>
    /// <param name="input">The input object.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="token">An optional cancellation token.</param>
    Task ApplyAsync(T input, IEngineContext context = null, CancellationToken token = default);

    /// <summary>
    ///     Serially apply the given inputs to the output object.
    /// </summary>
    /// <param name="inputs">The input objects.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="parallelizeInputs">Whether to parallelize on inputs.</param>
    /// /// <param name="token">An optional cancellation token.</param>
    Task ApplyAsync(IEnumerable<T> inputs, IEngineContext context = null, bool parallelizeInputs = false, CancellationToken token = default);

    /// <summary>
    ///     Stream inputs and apply them serially to the given output object.
    /// </summary>
    /// <param name="inputStream">The input object source.</param>
    /// <param name="context">An optional injected context.</param>
    /// <param name="token">An optional cancellation token.</param>
    Task ApplyAsync(IAsyncEnumerable<T> inputStream, IEngineContext context = null, CancellationToken token = default);
}