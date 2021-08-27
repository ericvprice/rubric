using RulesEngine.Rules;
using static System.String;

namespace RulesEngine.Builder;

internal class PreRuleBuilder<TIn, TOut> : IPreRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
  private readonly List<string> _deps;
  private readonly string _name;
  private readonly EngineBuilder<TIn, TOut> _parentBuilder;
  private readonly List<string> _provides;
  private Action<IEngineContext, TIn> _action;
  private Func<IEngineContext, TIn, bool> _predicate;

  internal PreRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name)
  {
    _parentBuilder = engineBuilder;
    _name = IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
    _provides = new List<string> { name };
    _deps = new List<string>();
  }


  public IEngineBuilder<TIn, TOut> EndRule()
  {
    _parentBuilder.Ruleset.AddPreRule(new LambdaRule<TIn>(_name, _predicate, _action, _deps, _provides));
    return _parentBuilder;
  }

  public IPreRuleBuilder<TIn, TOut> ThatProvides(string provides)
  {
    if (IsNullOrEmpty(provides)) throw new ArgumentException();
    _provides.Add(provides);
    return this;
  }

  public IPreRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TIn> action)
  {
    _action = action ?? throw new ArgumentNullException(nameof(action));
    return this;
  }

  public IPreRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
  {
    if (IsNullOrEmpty(dep)) throw new ArgumentException(dep);
    _deps.Add(dep);
    return this;
  }

  public IPreRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
  {
    _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
    return this;
  }

  public IPreRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, bool> predicate)
  {
    _predicate = predicate ?? throw new ArgumentNullException();
    return this;
  }
}
