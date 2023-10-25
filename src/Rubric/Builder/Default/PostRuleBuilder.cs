using Rubric.Rules;
using static System.String;

namespace Rubric.Builder.Default;

internal class PostRuleBuilder<TIn, TOut> : RuleBuilderBase, IPostRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Action<IEngineContext, TOut> _action;
  private Func<IEngineContext, TOut, bool> _predicate = (_, _) => true;
  
  internal PostRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name) => _parentBuilder = engineBuilder;

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, bool> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TOut> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    AddDependency(dep); 
    return this;
  }

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    AddProvides(provides);
    return this;
  }

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc/>
  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.Ruleset.AddPostRule(new LambdaRule<TOut>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}