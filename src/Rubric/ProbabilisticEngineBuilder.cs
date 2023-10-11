using Microsoft.Extensions.Logging;
using Rubric.Probabilistic.Builder;
using Rubric.Probabilistic.Builder.Default;

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

    public static Probabilistic.Async.Builder.IEngineBuilder<T> ForInputAsync<T>(ILogger logger = null)
        where T : class
        => new Probabilistic.Async.Builder.Default.EngineBuilder<T>(logger);

    public static Probabilistic.Async.Builder.IEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(ILogger logger = null)
        where TIn : class
        where TOut : class
        => new Probabilistic.Async.Builder.Default.EngineBuilder<TIn, TOut>(logger);

}
