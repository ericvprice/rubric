using System.Threading;
using Rubric.Rules.Async;
using static System.String;

namespace Rubric.Builder;

internal class AsyncPreRuleBuilder<TIn, TOut> : IAsyncPreRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  private readonly List<string> _deps;
  private readonly string _name;
  private readonly AsyncEngineBuilder<TIn, TOut> _parentBuilder;
  private readonly List<string> _provides;
  private Func<IEngineContext, TIn, CancellationToken, Task> _action;
  private Func<IEngineContext, TIn, CancellationToken, Task<bool>> _predicate;

  internal AsyncPreRuleBuilder(AsyncEngineBuilder<TIn, TOut> engineBuilder, string name)
  {
    _parentBuilder = engineBuilder;
    _name = IsNullOrEmpty(name) ? throw new ArgumentNullException(nameof(name)) : name;
    _provides = new() { name };
    _deps = new();
  }


  public IAsyncEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.AsyncRuleset.AddAsyncPreRule(
        new LambdaAsyncRule<TIn>(_name, _predicate, _action, _deps, _provides));
    return _parentBuilder;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    if (IsNullOrEmpty(provides)) throw new ArgumentException(provides);
    _provides.Add(provides);
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, Task> action)
  {
    if (action == null)
    {
      throw new ArgumentNullException(nameof(action));
    }
    _action = (ctx, inObj, _) => action(ctx, inObj);
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, CancellationToken, Task> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    if (IsNullOrEmpty(dep)) throw new ArgumentNullException(nameof(dep));
    _deps.Add(dep);
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, Task<bool>> predicate)
  {
    if (predicate == null)
      throw new ArgumentNullException(nameof(predicate));
    _predicate = (ctx, inObj, _) => predicate(ctx, inObj);
    return this;
  }

  public IAsyncPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, CancellationToken, Task<bool>> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }
}
