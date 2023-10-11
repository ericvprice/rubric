using Rubric.Rules.Probabilistic.Async;
using static System.String;

namespace Rubric.Probabilistic.Async.Builder.Default;

internal class PostRuleBuilder<TIn, TOut> : IPostRuleBuilder<TIn, TOut>
    where TIn : class
    where TOut : class
{
    private readonly List<string> _deps;
    private readonly string _name;
    private readonly EngineBuilder<TIn, TOut> _parentBuilder;
    private readonly List<string> _provides;
    private Func<IEngineContext, TOut, CancellationToken, Task> _action;
    private Func<IEngineContext, TOut, CancellationToken, Task<double>> _predicate;

    internal PostRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name)
    {
        _parentBuilder = engineBuilder;
        _name = IsNullOrWhiteSpace(name) ? throw new ArgumentException(null, nameof(name)) : name;
        _provides = new() { name };
        _deps = new();
    }

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
        if (IsNullOrWhiteSpace(dep)) throw new ArgumentException(null, nameof(dep));
        _deps.Add(dep);
        return this;
    }

    /// <inheritdoc />
    public IPostRuleBuilder<TIn, TOut> ThatDependsOn(Type dep)
    {
        _deps.Add(dep?.FullName ?? throw new ArgumentNullException(nameof(dep)));
        return this;
    }

    /// <inheritdoc />
    public IPostRuleBuilder<TIn, TOut> ThatProvides(string provides)
    {
        if (IsNullOrWhiteSpace(provides)) throw new ArgumentException(null, nameof(provides));
        _provides.Add(provides);
        return this;
    }

    /// <inheritdoc />
    public IEngineBuilder<TIn, TOut> EndRule()
    {
        _parentBuilder.AsyncRuleset.AddPostRule(
          new LambdaRule<TOut>(_name, _predicate, _action, _deps, _provides));
        return _parentBuilder;
    }
}