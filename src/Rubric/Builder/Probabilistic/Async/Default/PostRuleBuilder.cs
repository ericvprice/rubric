using Rubric.Rules.Probabilistic.Async;
using static System.String;

namespace Rubric.Builder.Probabilistic.Async.Default;

internal class PostRuleBuilder<TIn, TOut> : RuleBuilderBase, IPostRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private Func<IEngineContext, TOut, CancellationToken, Task> _action;
  private Func<IEngineContext, TOut, CancellationToken, Task<double>> _predicate;

  internal PostRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name) : base(name) => _parentBuilder = engineBuilder;

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, Task<double>> predicate)
  {
    if (predicate == null)
      throw new ArgumentNullException(nameof(predicate));
    _predicate = (ctx, outObj, _) => predicate(ctx, outObj);
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, CancellationToken, Task<double>> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TOut, Task> action)
  {
    if (action == null)
      throw new ArgumentNullException(nameof(action));
    _action = (context, outObj, _) => action(context, outObj);
    return this;
  }

  /// <inheritdoc />
  public IPostRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TOut, CancellationToken, Task> action)
  {
    _action = action ?? throw new ArgumentNullException(null, nameof(Action));
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

  /// <inheritdoc/>
  public IPostRuleBuilder<TIn, TOut> WithCaching(PredicateCaching caching)
  {
    Caching = caching;
    return this;
  }

  /// <inheritdoc />
  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.AsyncRuleset.AddPostRule(
      new LambdaRule<TOut>(Name, _predicate, _action, Dependencies, Provides));
    return _parentBuilder;
  }
}