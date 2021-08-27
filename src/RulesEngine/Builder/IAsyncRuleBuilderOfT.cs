namespace RulesEngine.Builder
{
  public interface IAsyncRuleBuilder<T>
      where T : class
  {
    IAsyncRuleBuilder<T> WithPredicate(Func<IEngineContext, T, Task<bool>> predicate);

    IAsyncRuleBuilder<T> WithPredicate(Func<IEngineContext, T, CancellationToken, Task<bool>> predicate);

    IAsyncRuleBuilder<T> WithAction(Func<IEngineContext, T, Task> action);

    IAsyncRuleBuilder<T> WithAction(Func<IEngineContext, T, CancellationToken, Task> action);

    IAsyncRuleBuilder<T> ThatDependsOn(string dep);

    IAsyncRuleBuilder<T> ThatDependsOn(Type dep);

    IAsyncRuleBuilder<T> ThatProvides(string provides);

    IAsyncEngineBuilder<T> EndRule();
  }
}