using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Builder.Probabilistic.Async.Implementation;

/// <inheritdoc cref="IPreRuleBuilder{TIn,TOut}"/>
internal sealed class PreRuleBuilder<TIn, TOut> : RuleBuilderBase, IPreRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Func<IEngineContext, TIn, CancellationToken, Task> _action;
  private Func<IEngineContext, TIn, CancellationToken, Task<double>> _predicate;

  internal PreRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name)
    => _parentBuilder = engineBuilder;

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, Task<double>> predicate)
  {
    if (predicate == null)
      throw new ArgumentNullException(nameof(predicate));
    _predicate = (ctx, inObj, _) => predicate(ctx, inObj);
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, CancellationToken, Task<double>> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, Task> action)
  {
    if (action == null) throw new ArgumentNullException(nameof(action));
    _action = (ctx, inObj, _) => action(ctx, inObj);
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, CancellationToken, Task> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    AddProvides(provides);
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.AsyncRuleset.AddPreRule(
      new LambdaRule<TIn>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}