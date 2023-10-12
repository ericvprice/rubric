using Microsoft.Extensions.Logging;
using Rubric.Builder.Probabilistic;
using Rubric.Builder.Probabilistic.Default;

namespace Rubric;

public static class ProbabilisticEngineBuilder
{

    public static IEngineBuilder<T> ForInput<T>(ILogger logger = null)
        where T : class
        => new EngineBuilder<T>(logger);

    public static IEngineBuilder<TIn, TOut> ForInputAndOutput<TIn, TOut>(ILogger logger = null)
      where TIn : class
      where TOut : class
      => new EngineBuilder<TIn, TOut>(logger);

    public static Builder.Probabilistic.Async.IEngineBuilder<T> ForInputAsync<T>(ILogger logger = null)
        where T : class
        => new Builder.Probabilistic.Async.Default.EngineBuilder<T>(logger);

    public static Builder.Probabilistic.Async.IEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(ILogger logger = null)
        where TIn : class
        where TOut : class
        => new Builder.Probabilistic.Async.Default.EngineBuilder<TIn, TOut>(logger);

}
