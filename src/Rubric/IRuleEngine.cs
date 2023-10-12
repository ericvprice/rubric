using Microsoft.Extensions.Logging;

namespace Rubric;

/// <summary>
///     Basic interface for rule engines.
/// </summary>
public interface IRuleEngine
{
    /// <summary>
    ///     Logger instance.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    ///     Whether this engine is async.
    /// </summary>
    bool IsAsync { get; }

    /// <summary>
    ///     The input type for this engine.
    /// </summary>
    Type InputType { get; }

    /// <summary>
    ///     The output type for this engine.
    /// </summary>
    Type OutputType { get; }

    /// <summary>
    ///   In-engine exception handler.
    /// </summary>
    IExceptionHandler ExceptionHandler { get; }

}
