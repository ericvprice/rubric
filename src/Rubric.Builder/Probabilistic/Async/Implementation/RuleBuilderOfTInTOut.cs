using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Builder.Probabilistic.Async.Implementation;

/// <inheritdoc cref="IRuleBuilder{TIn,TOut}"/>
internal sealed class RuleBuilder<TIn, TOut> : RuleBuilderBase, IRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Func<IEngineContext, TIn, TOut, CancellationToken, Task> _action;
  private Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> _predicate;

  internal RuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name)
    => _parentBuilder = engineBuilder;

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, Task<double>> predicate)
  {
    if (predicate == null)
      throw new ArgumentNullException(nameof(predicate));
    _predicate = (ctx, inObj, outObj, _) => predicate(ctx, inObj, outObj);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithPredicate(
    Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, Task> action)
  {
    if (action == null)
      throw new ArgumentNullException(nameof(action));
    _action = (ctx, inObj, outObj, _) => action(ctx, inObj, outObj);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, CancellationToken, Task> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    AddProvides(provides);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.AsyncRuleset.AddRule(
      new LambdaRule<TIn, TOut>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}