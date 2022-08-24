namespace Rubric.Builder;

public interface IRuleBuilder<T>
  where T : class
{
  IRuleBuilder<T> WithPredicate(Func<IEngineContext, T, bool> predicate);

  IRuleBuilder<T> WithAction(Action<IEngineContext, T> action);

  IRuleBuilder<T> ThatDependsOn(string dep);

  IRuleBuilder<T> ThatDependsOn(Type dep);

  IRuleBuilder<T> ThatProvides(string provides);

  IEngineBuilder<T> EndRule();
}