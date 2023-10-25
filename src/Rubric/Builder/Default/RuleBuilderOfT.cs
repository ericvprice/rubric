using Rubric.Rules;
using static System.String;

namespace Rubric.Builder.Default;

internal class RuleBuilder<T> : RuleBuilderBase, IRuleBuilder<T>
    where T : class
{
  private readonly EngineBuilder<T> _parentBuilder;
  private Func<IEngineContext, T, bool> _predicate = (_, _) => true;
  private Action<IEngineContext, T> _action;
  
  internal RuleBuilder(EngineBuilder<T> engineBuilder, string name) : base(name) => _parentBuilder = engineBuilder;

  /// <inheritdoc/>
  public IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, bool> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc/>
  public IRuleBuilder<T> WithAction(Action<IEngineContext, T> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc/>
  public IRuleBuilder<T> ThatDependsOn(string dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc/>
  public IRuleBuilder<T> ThatDependsOn(Type dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc/>
  public IRuleBuilder<T> ThatProvides(string provides)
  {
    AddProvides(provides); return this;
  }

  /// <inheritdoc/>
  public IRuleBuilder<T> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc/>
  public IEngineBuilder<T> EndRule()
  {
    _parentBuilder.Ruleset.AddRule(new LambdaRule<T>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _parentBuilder;
  }
}