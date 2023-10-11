using static System.String;

namespace Rubric.Rules.Probabilistic;

/// <summary>
///     A runtime-constructed processing rule.
/// </summary>
/// <typeparam name="TIn">The engine input type.</typeparam>
/// <typeparam name="TOut">The engine output type.</typeparam>
public class LambdaRule<TIn, TOut> : IRule<TIn, TOut>
{
    private readonly Action<IEngineContext, TIn, TOut> _action;

    private readonly Func<IEngineContext, TIn, TOut, double> _predicate;

    public LambdaRule(
        string name,
        Func<IEngineContext, TIn, TOut, double> predicate,
        Action<IEngineContext, TIn, TOut> action,
        IEnumerable<string> dependencies = null,
        IEnumerable<string> provides = null
    )
    {
        Name = IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name is required and must be nonempty.", nameof(name))
            : name;
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _action = action ?? throw new ArgumentNullException(nameof(action));
        Dependencies = dependencies?.ToArray() ?? Array.Empty<string>();
        Provides = provides?.ToArray() ?? Array.Empty<string>();
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IEnumerable<string> Dependencies { get; }

    /// <inheritdoc/>
    public IEnumerable<string> Provides { get; }

    /// <inheritdoc/>
    public void Apply(IEngineContext context, TIn input, TOut output)
        => _action(context, input, output);

    /// <inheritdoc/>
    public double DoesApply(IEngineContext context, TIn input, TOut output)
        => _predicate(context, input, output);
}