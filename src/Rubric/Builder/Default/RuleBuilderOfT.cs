using Rubric.Rules;
using static System.String;

namespace Rubric.Builder.Default;

internal class RuleBuilder<T> : IRuleBuilder<T>
    where T : class
{
    private readonly EngineBuilder<T> _parentBuilder;
    private readonly string _name;
    private readonly List<string> _provides;
    private readonly List<string> _deps = new();
    private Func<IEngineContext, T, bool> _predicate = (_, _) => true;
    private Action<IEngineContext, T> _action;

    internal RuleBuilder(EngineBuilder<T> engineBuilder, string name)
    {
        _name = IsNullOrEmpty(name) ? throw new ArgumentException(null, nameof(name)) : name;
        _parentBuilder = engineBuilder;
        _provides = new() { name };
    }

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
        if (IsNullOrEmpty(dep)) throw new ArgumentException(null, nameof(dep));
        _deps.Add(dep);
        return this;
    }

    /// <inheritdoc/>
    public IRuleBuilder<T> ThatDependsOn(Type dep)
    {
        _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
        return this;
    }

    /// <inheritdoc/>
    public IRuleBuilder<T> ThatProvides(string provides)
    {
        if (IsNullOrEmpty(provides)) throw new ArgumentException(null, nameof(provides));
        _provides.Add(provides);
        return this;
    }

    /// <inheritdoc/>
    public IEngineBuilder<T> EndRule()
    {
        _parentBuilder.Ruleset.AddRule(new LambdaRule<T>(_name, _predicate, _action, _deps, _provides));
        return _parentBuilder;
    }
}