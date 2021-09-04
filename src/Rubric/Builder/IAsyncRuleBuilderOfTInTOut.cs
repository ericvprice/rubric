namespace Rubric.Builder
{
  public interface IAsyncRuleBuilder<TIn, TOut>
      where TIn : class
      where TOut : class
  {
    IAsyncRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, Task<bool>> predicate);

    IAsyncRuleBuilder<TIn, TOut> WithPredicate(Func<IEngineContext, TIn, TOut, CancellationToken, Task<bool>> predicate);

    IAsyncRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, Task> action);

    IAsyncRuleBuilder<TIn, TOut> WithAction(Func<IEngineContext, TIn, TOut, CancellationToken, Task> action);

    IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(string dep);

    IAsyncRuleBuilder<TIn, TOut> ThatDependsOn(Type dep);

    IAsyncRuleBuilder<TIn, TOut> ThatProvides(string provides);

    IAsyncEngineBuilder<TIn, TOut> EndRule();
  }
}