using Microsoft.Extensions.Logging;
using Rubric.Builder.Async;

namespace Rubric.Builder;

public static class EngineBuilder
{
  public static IEngineBuilder<TIn, TOut> ForInputAndOutput<TIn, TOut>(ILogger logger = null)
      where TIn : class
      where TOut : class
      => new EngineBuilder<TIn, TOut>(logger);

  public static IEngineBuilder<T> ForInput<T>(ILogger logger = null)
      where T : class
      => new EngineBuilder<T>(logger);

  public static IAsyncEngineBuilder<T> ForInputAsync<T>(ILogger logger = null)
      where T : class
      => new AsyncEngineBuilder<T>(logger);


  public static IAsyncEngineBuilder<TIn, TOut> ForInputAndOutputAsync<TIn, TOut>(ILogger logger = null)
      where TIn : class
      where TOut : class
      => new AsyncEngineBuilder<TIn, TOut>(logger);
}
