using Rubric.Rules;

namespace Rubric.Builder.Implementation;

/// <inheritdoc cref="IRuleBuilder{TIn, TOut}"/>
internal class RuleBuilder<TIn, TOut> : RuleBuilderBase, IRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Action<IEngineContext, TIn, TOut> _action;
  private Func<IEngineContext, TIn, TOut, bool> _predicate = (_, _, _) => true;

  internal RuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name)
    => _parentBuilder = engineBuilder;


  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, bool> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn, TOut> action)
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
    _parentBuilder.Ruleset.AddRule(
      new LambdaRule<TIn, TOut>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}