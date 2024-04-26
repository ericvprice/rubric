using Rubric.Rules;

namespace Rubric.Builder.Implementation;

/// <inheritdoc cref="IPreRuleBuilder{TIn,TOut}"/>/>
internal sealed class PreRuleBuilder<TIn, TOut> : RuleBuilderBase, IPreRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Action<IEngineContext, TIn> _action;
  private Func<IEngineContext, TIn, bool> _predicate = (_, _) => true;

  internal PreRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name)
    => _parentBuilder = engineBuilder;

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, bool> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IPreRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn> action)
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
    _parentBuilder.Ruleset.AddPreRule(new LambdaRule<TIn>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}