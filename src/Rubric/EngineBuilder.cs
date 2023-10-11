using Microsoft.Extensions.Logging;
using Rubric.Builder;
using Rubric.Builder.Default;

namespace Rubric;

public static class EngineBuilder
{

    public static IEngineBuilder<T> ForInput<T>(ILogger logger = null)
        where T : class
        => new EngineBuilder<T>(logger);

    public static IEngineBuilder<TIn, TOut> ForInputAndOutput<TIn, TOut>(ILogger logger = null)
      where TIn : class
      where TOut : class
      => new EngineBuilder<TIn, TOut>(logger);

    public static Async.Builder.IEngineBuilder<T> ForInputAsync<T>(ILogger logger = null)
        where T : class
        => new Async.Builder.Default.EngineBuilder<T>(logger);

    public static Async.Builder.IEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(ILogger logger = null)
        where TIn : class
        where TOut : class
        => new Async.Builder.Default.EngineBuilder<TIn, TOut>(logger);

}
