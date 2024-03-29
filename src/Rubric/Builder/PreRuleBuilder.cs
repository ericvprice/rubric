using Rubric.Rules;
using static System.String;

namespace Rubric.Builder;

internal class PreRuleBuilder<TIn, TOut> : IPreRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  private readonly List<string> _deps;
  private readonly string _name;
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private readonly List<string> _provides;
  private Action<IEngineContext, TIn> _action;
  private Func<IEngineContext, TIn, bool> _predicate = (_, _) => true;

  internal PreRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name)
  {
    _parentBuilder = engineBuilder;
    _name = IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
    _provides = new() { name };
    _deps = new();
  }

  /// <inheritdoc/>
  public IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, bool> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    return this;
  }

  /// <inheritdoc/>
  public IPreRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  /// <inheritdoc/>
  public IPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    if (IsNullOrEmpty(dep)) throw new ArgumentException(null, nameof(dep));
    _deps.Add(dep);
    return this;
  }

  /// <inheritdoc/>
  public IPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
    return this;
  }

  /// <inheritdoc/>
  public IPreRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    if (IsNullOrEmpty(provides)) throw new ArgumentException(null, nameof(provides));
    _provides.Add(provides);
    return this;
  }

  /// <inheritdoc/>
  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.Ruleset.AddPreRule(new LambdaRule<TIn>(_name, _predicate, _action, _deps, _provides));
    return _parentBuilder;
  }
}
