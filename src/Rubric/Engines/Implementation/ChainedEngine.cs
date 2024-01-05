using Microsoft.Extensions.Logging;
using Rubric.Rules;

namespace Rubric.Engines.Implementation;

internal class ChainedEngine<TIn, TInt, TOut>(IRuleEngine<TIn, TInt> first,
                                              IRuleEngine<TInt, TOut> second,
                                              Func<TInt> intFactory)
  : IRuleEngine<TIn, TOut>
  where TIn : class
  where TInt : class
  where TOut : class
{

    private readonly Func<TInt> _intFactory = intFactory ?? throw new ArgumentNullException(nameof(intFactory));


    public IRuleEngine<TIn, TInt> First { get; } = first ?? throw new ArgumentNullException(nameof(first));

    public IRuleEngine<TInt, TOut> Second { get; } = second ?? throw new ArgumentNullException(nameof(second));


    /// <inheritdoc />
    public ILogger Logger => throw new NotImplementedException();

    /// <inheritdoc />
    public bool IsAsync => false;

    /// <inheritdoc />
    public Type InputType => typeof(TIn);

    /// <inheritdoc />
    public Type OutputType => typeof(TOut);

    /// <inheritdoc />
    public IExceptionHandler ExceptionHandler => throw new NotImplementedException();

    /// <inheritdoc />
    public IEnumerable<IRule<TIn>> PreRules => First.PreRules;

    /// <inheritdoc />
    public IEnumerable<IRule<TIn, TOut>> Rules => throw new NotImplementedException();

    /// <inheritdoc />
    public IEnumerable<IRule<TOut>> PostRules => Second.PostRules;

    /// <inheritdoc />
    public void Apply(TIn input, TOut output, IEngineContext context = null)
    {
        var intermediate = _intFactory();
        context ??= new EngineContext();
        First.Apply(input, intermediate, context);
        Second.Apply(intermediate, output, context);
    }

    /// <inheritdoc />
    public void Apply(IEnumerable<TIn> inputs, TOut output, IEngineContext context = null)
    {
        var intermediate = _intFactory();
        context ??= new EngineContext();
        First.Apply(inputs, intermediate, context);
        Second.Apply(intermediate, output, context);
    }
}