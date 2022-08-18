namespace Rubric.Rules;

/// <summary>
///   A runtime-constructed processing rule.
/// </summary>
/// <typeparam name="T">The engine input type.</typeparam>
public class LambdaRule<T> : IRule<T>
{
  private readonly Action<IEngineContext, T> _action;

  private readonly Func<IEngineContext, T, bool> _predicate;

  public LambdaRule(
    string name,
    Func<IEngineContext, T, bool> predicate,
    Action<IEngineContext, T> action,
    IEnumerable<string> dependencies = null,
    IEnumerable<string> provides = null
  )
  {
    Name = string.IsNullOrWhiteSpace(name)
      ? throw new ArgumentException("Name is required and must be nonempty.", nameof(name))
      : name;
    _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    _action = action ?? throw new ArgumentNullException(nameof(action));
    Dependencies = dependencies?.ToArray() ?? Array.Empty<string>();
    Provides = provides?.ToArray() ?? Array.Empty<string>();
  }
  
  public string Name { get; }

  public IEnumerable<string> Dependencies { get; }

  public IEnumerable<string> Provides { get; }

  public void Apply(IEngineContext context, T input)
    => _action(context, input);

  public bool DoesApply(IEngineContext context, T input)
    => _predicate(context, input);
}