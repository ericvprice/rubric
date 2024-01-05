using Rubric.Engines.Implementation;

namespace Rubric;

internal static class EngineExtensions
{
  public static Engines.IRuleEngine<TIn, TOut> Chain<TIn, TInt, TOut>(
    this Engines.IRuleEngine<TIn, TInt> first,
    Func<TInt> factory,
    Engines.IRuleEngine<TInt, TOut> second)
    where TIn : class
    where TInt : class
    where TOut : class
    => new ChainedEngine<TIn, TInt, TOut>(first, second, factory);

  public static Engines.Async.IRuleEngine<TIn, TOut> Chain<TIn, TInt, TOut>(
    this Engines.Async.IRuleEngine<TIn, TInt> first,
    Func<TInt> factory, 
    Engines.Async.IRuleEngine<TInt, TOut> second)
    where TIn : class
    where TInt : class
    where TOut : class
    => new Engines.Async.Implementation.ChainedEngine<TIn, TInt, TOut>(first, second, factory);
}