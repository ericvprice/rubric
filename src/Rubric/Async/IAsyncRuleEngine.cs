namespace Rubric.Async;

public interface IAsyncRuleEngine : IRuleEngine
{
  /// <summary>
  ///     Whether this engine is executing rules in parallel.
  /// </summary>
  bool IsParallel { get; }

}