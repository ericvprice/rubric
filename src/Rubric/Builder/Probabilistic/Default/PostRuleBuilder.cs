using Rubric.Builder.Probabilistic;
using Rubric.Rules.Probabilistic;
using static System.String;

namespace Rubric.Builder.Probabilistic.Default;

internal class PostRuleBuilder<TIn, TOut> : IPostRuleBuilder<TIn, TOut>
  where TIn : class
  where TOut : class
{
    private readonly List<string> _deps;
    private readonly EngineBuilder<TIn, TOut> _parentBuilder;
    private readonly List<string> _provides;
    private Action<IEngineContext, TOut> _action;
    private Func<IEngineContext, TOut, double> _predicate = (_, _) => 1D;

    internal PostRuleBuilder(EngineBuilder<TIn, TOut> engineBuilder, string name)
    {
        _parentBuilder = engineBuilder;
        Name = IsNullOrWhiteSpace(name)
          ? throw new ArgumentException("String cannot be null or empty.", nameof(name))
          : name;
        _provides = new() { name };
        _deps = new();
    }

    internal string Name { get; }

    /// <inheritdoc/>
    public IPostRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TOut, double> predicate)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        return this;
    }

    /// <inheritdoc/>
    public IPostRuleBuilder<TIn, TOut> WithAction(Action<IEngineContext, TOut> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        return this;
    }

    /// <inheritdoc/>
    public IPostRuleBuilder<TIn, TOut> ThatDependsOn(string dep)
    {
        if (IsNullOrWhiteSpace(dep)) throw new ArgumentException("String cannot be null or empty.", nameof(dep));
        _deps.Add(dep);
        return this;
    }

    /// <inheritdoc/>
    public IPostRuleBuilder<TIn, TOut> ThatDependsOn(Type type)
    {
        _deps.Add(type?.FullName ?? throw new ArgumentNullException(nameof(type)));
        return this;
    }

    /// <inheritdoc/>
    public IPostRuleBuilder<TIn, TOut> ThatProvides(string provides)
    {
        if (IsNullOrWhiteSpace(provides))
            throw new ArgumentException("String cannot be null or empty.", nameof(provides));
        _provides.Add(provides);
        return this;
    }

    /// <inheritdoc/>
    public IEngineBuilder<TIn, TOut> EndRule()
    {
        _parentBuilder.Ruleset.AddPostRule(new LambdaRule<TOut>(Name, _predicate, _action, _deps, _provides));
        return _parentBuilder;
    }
}