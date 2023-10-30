using Rubric.Rules.Probabilistic;

namespace Rubric.Builder.Probabilistic.Implementation;

/// <inheritdoc cref="IPostRuleBuilder{TIn,TOut}"/>
internal class PostRuleBuilder<TIn, TOut> : RuleBuilderBase, IPostRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Action<IEngineContext, TOut> _action;
  private Func<IEngineContext, TOut, double> _predicate = (_, _) => 1D;

  internal PostRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name)
    => _parentBuilder = engineBuilder;

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, double> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TOut> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    AddProvides(provides);
    return this;
  }

  public IPostRuleBuilder<TIn, TOut> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.Ruleset.AddPostRule(
      new LambdaRule<TOut>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}