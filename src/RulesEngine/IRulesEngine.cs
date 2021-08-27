using Microsoft.Extensions.Logging;

namespace RulesEngine;

/// <summary>
///     Basic interface for rule engines.
/// </summary>
public interface IRulesEngine
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
  ///     Whether this engine is executing rules in parallel.
  /// </summary>
  bool IsParallel { get; }

  /// <summary>
  ///     The input type for this engine.
  /// </summary>
  Type InputType { get; }

  /// <summary>
  ///     The output type for this engine.
  /// </summary>
  Type OutputType { get; }

  IExceptionHandler ExceptionHandler { get; }

  EngineException LastException { get; }
}