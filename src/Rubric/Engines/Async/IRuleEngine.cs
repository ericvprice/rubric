namespace Rubric.Engines.Async;

/// <summary>
///   A base interface for all rule engines.
/// </summary>
public interface IRuleEngine : Rubric.IRuleEngine
{
    /// <summary>
    ///     Whether this engine is executing rules in parallel.
    /// </summary>
    bool IsParallel { get; }

}