// namespace RulesEngine.Rules.Async;

// public class LambdaAsyncRule<TIn, TOut> : IAsyncRule<TIn, TOut>
// {
//   private readonly Func<IEngineContext, TIn, TOut, Task> _body;
//   private readonly Func<IEngineContext, TIn, TOut, Task<bool>> _predicate;

//   public LambdaAsyncRule(
//       string name,
//       Func<IEngineContext, TIn, TOut, Task<bool>> predicate,
//       Func<IEngineContext, TIn, TOut, Task> body,
//       IEnumerable<string> dependencies = null,
//       IEnumerable<string> provides = null
//   )
//   {
//     Name = name ?? throw new ArgumentNullException(nameof(name));
//     _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
//     _body = body ?? throw new ArgumentNullException(nameof(body));
//     Dependencies = dependencies?.ToArray() ?? new string[0];
//     Provides = provides?.ToArray() ?? new string[0];
//   }


//   public string Name { get; }

//   public IEnumerable<string> Dependencies { get; }

//   public IEnumerable<string> Provides { get; }

//   public Task Apply(IEngineContext context, TIn input, TOut output)
//       => _body(context, input, output);

//   public Task<bool> DoesApply(IEngineContext context, TIn input, TOut output)
//       => _predicate(context, input, output);
// }

// public class LambdaAsyncRule<T> : IAsyncRule<T>
// {
//   private readonly Func<IEngineContext, T, Task> _body;
//   private readonly Func<IEngineContext, T, Task<bool>> _predicate;

//   public LambdaAsyncRule(
//       string name,
//       Func<IEngineContext, T, Task<bool>> predicate,
//       Func<IEngineContext, T, Task> body,
//       IEnumerable<string> dependencies = null,
//       IEnumerable<string> provides = null
//   )
//   {
//     Name = string.IsNullOrEmpty(name) ? throw new ArgumentException(nameof(name)) : name;
//     _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
//     _body = body ?? throw new ArgumentNullException(nameof(body));
//     Dependencies = dependencies?.ToArray() ?? new string[0];
//     Provides = provides?.ToArray() ?? new string[0];
//   }


//   public string Name { get; }

//   public IEnumerable<string> Dependencies { get; }

//   public IEnumerable<string> Provides { get; }

//   public Task Apply(IEngineContext context, T input)
//       => _body(context, input);

//   public Task<bool> DoesApply(IEngineContext context, T input)
//       => _predicate(context, input);
// }
