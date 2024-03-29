using Rubric.Rules.Async;
using static System.String;

namespace Rubric.Builder.Async;

internal class AsyncRuleBuilder<T> : IAsyncRuleBuilder<T>
    where T : class
{

  private readonly AsyncEngineBuilder<T> _builder;
  private readonly List<string> _deps;
  private readonly string _name;
  private readonly List<string> _provides;
  private Func<IEngineContext, T, CancellationToken, Task> _action;
  private Func<IEngineContext, T, CancellationToken, Task<bool>> _predicate = (_, _, _) => Task.FromResult(true);

  internal AsyncRuleBuilder(AsyncEngineBuilder<T> builder, string name)
  {
    _builder = builder;
    _name = IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
    _provides = new() { name };
    _deps = new();
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> WithPredicate(Func<IEngineContext, T, Task<bool>> predicate)
  {
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    _predicate = (ctx, inObj, _) => predicate(ctx, inObj);
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> WithPredicate(Func<IEngineContext, T, CancellationToken, Task<bool>> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> WithAction(Func<IEngineContext, T, Task> action)
  {
    if (action == null) throw new ArgumentNullException(nameof(action));
    _action = (ctx, inObj, _) => action(ctx, inObj);
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> WithAction(Func<IEngineContext, T, CancellationToken, Task> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> ThatDependsOn(string dep)
  {
    if (IsNullOrEmpty(dep)) throw new ArgumentException(null, nameof(dep));
    _deps.Add(dep);
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> ThatProvides(string provides)
  {
    if (IsNullOrEmpty(provides)) throw new ArgumentException(null, nameof(provides));
    _provides.Add(provides);
    return this;
  }

  /// <inheritdoc />
  public IAsyncRuleBuilder<T> ThatDependsOn(Type dep)
  {
    _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
    return this;
  }

  /// <inheritdoc />
  public IAsyncEngineBuilder<T> EndRule()
  {
    _builder.AsyncRuleset.AddAsyncRule(new LambdaAsyncRule<T>(_name, _predicate, _action, _deps, _provides));
    return _builder;
  }
}
