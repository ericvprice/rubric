namespace Rubric.Async;

public interface IRuleEngine : Rubric.IRuleEngine
{
  /// <summary>
  ///     Whether this engine is executing rules in parallel.
  /// </summary>
  bool IsParallel { get; }

}