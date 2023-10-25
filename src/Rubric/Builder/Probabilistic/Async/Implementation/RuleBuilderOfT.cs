using Rubric.Rules.Probabilistic.Async;

namespace Rubric.Builder.Probabilistic.Async.Implementation;

internal class RuleBuilder<T> : RuleBuilderBase, IRuleBuilder<T>
  where T : class
{
  private readonly EngineBuilder<T> _builder;
  private Func<IEngineContext, T, CancellationToken, Task> _action;
  private Func<IEngineContext, T, CancellationToken, Task<double>> _predicate = (_, _, _) => Task.FromResult(1D);

  internal RuleBuilder(EngineBuilder<T> builder, string name) : base(name) => _builder = builder;

  /// <inheritdoc />
  public IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, Task<double>> predicate)
  {
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    _predicate = (ctx, inObj, _) => predicate(ctx, inObj);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, CancellationToken, Task<double>> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> WithAction(Func<IEngineContext, T, Task> action)
  {
    if (action == null) throw new ArgumentNullException(nameof(action));
    _action = (ctx, inObj, _) => action(ctx, inObj);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> WithAction(Func<IEngineContext, T, CancellationToken, Task> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> ThatDependsOn(string dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> ThatProvides(string provides)
  {
    AddProvides(provides);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> ThatDependsOn(Type dep)
  {
    AddDependency(dep);
    return this;
  }

  /// <inheritdoc />
  public IRuleBuilder<T> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<T> EndRule()
  {
    _builder.AsyncRuleset.AddRule(new LambdaRule<T>(Name, _predicate, _action, Dependencies, Provides, Caching));
    return _builder;
  }
}