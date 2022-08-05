using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rubric.Rules;

namespace Rubric.Builder;

internal class EngineBuilder<TIn, TOut> : IEngineBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal Ruleset<TIn, TOut> Ruleset { get; } = new();

  internal ILogger Logger { get; }

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  public IPostRuleBuilder<TIn, TOut> WithPostRule(string name)
      => new PostRuleBuilder<TIn, TOut>(this, name);

  public IPreRuleBuilder<TIn, TOut> WithPreRule(string name)
      => new PreRuleBuilder<TIn, TOut>(this, name);

  public IRuleBuilder<TIn, TOut> WithRule(string name)
      => new RuleBuilder<TIn, TOut>(this, name);

  public IEngineBuilder<TIn, TOut> WithPostRule(IRule<TOut> rule)
  {
    Ruleset.AddPostRule(rule);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithPreRule(IRule<TIn> rule)
  {
    Ruleset.AddPreRule(rule);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithRule(IRule<TIn, TOut> rule)
  {
    Ruleset.AddRule(rule);
    return this;
  }

  public IEngineBuilder<TIn, TOut> WithExceptionHandler(IExceptionHandler h)
  {
    ExceptionHandler = h;
    return this;
  }

  public IRuleEngine<TIn, TOut> Build() => new RuleEngine<TIn, TOut>(Ruleset, ExceptionHandler, Logger);
}

internal class EngineBuilder<T> : IEngineBuilder<T>
    where T : class
{
  internal EngineBuilder(ILogger logger = null) => Logger = logger ?? NullLogger.Instance;

  internal Ruleset<T> Ruleset { get; } = new();

  internal ILogger Logger { get; }

  public IExceptionHandler ExceptionHandler { get; private set; } = ExceptionHandlers.Rethrow;

  public IRuleBuilder<T> WithRule(string name)
      => new RuleBuilder<T>(this, name);

  public IEngineBuilder<T> WithRule(IRule<T> rule)
  {
    Ruleset.AddRule(rule);
    return this;
  }

  public IEngineBuilder<T> WithExceptionHandler(IExceptionHandler h)
  {
    ExceptionHandler = h;
    return this;
  }

  public IRuleEngine<T> Build() => new RuleEngine<T>(Ruleset, ExceptionHandler, Logger);
}

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
