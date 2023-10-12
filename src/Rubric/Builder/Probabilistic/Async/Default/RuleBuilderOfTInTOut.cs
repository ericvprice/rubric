using Rubric.Builder.Probabilistic.Async;
using Rubric.Rules.Probabilistic.Async;
using static System.String;

namespace Rubric.Builder.Probabilistic.Async.Default;

internal class RuleBuilder<TIn, TOut> : IRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
    private readonly List<string> _deps;
    private readonly string _name;

    private readonly EngineBuilder<TIn, TOut> _parentBuilder;
    private readonly List<string> _provides;
    private Func<IEngineContext, TIn, TOut, CancellationToken, Task> _action;
    private Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> _predicate;

    internal RuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name)
    {
        _parentBuilder = engineBuilder;
        _name = IsNullOrEmpty(name) ? throw new ArgumentNullException(null, nameof(name)) : name;
        _provides = new() { name };
        _deps = new();
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, Task<double>> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));
        _predicate = (ctx, inObj, outObj, _) => predicate(ctx, inObj, outObj);
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, CancellationToken, Task<double>> predicate)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, Task> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        _action = (ctx, inObj, outObj, _) => action(ctx, inObj, outObj);
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, CancellationToken, Task> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
    {
        if (IsNullOrEmpty(dep)) throw new ArgumentNullException(nameof(dep));
        _deps.Add(dep);
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
    {
        _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
        return this;
    }

    /// <inheritdoc />
    public IRuleBuilder<TIn, TOut> ThatProvides(string provides)
    {
        if (IsNullOrEmpty(provides)) throw new ArgumentNullException(nameof(provides));
        _provides.Add(provides);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> EndRule()
    {
        _parentBuilder.AsyncRuleset.AddRule(
          new LambdaRule<TIn, TOut>(_name, _predicate, _action, _deps, _provides));
        return _parentBuilder;
    }
}