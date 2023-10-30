namespace Rubric.Tests;

public class EngineContextExtensionTests
{
  [Fact]
  public void EngineContextExtensionsExceptions()
  {
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.ClearExecutionPredicateCache(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.ClearInputPredicateCache(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetExecutionPredicateCache(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetInputPredicateCache(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.SetExecutionInfo(null, null, ""));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.ClearLastException(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.ClearAllCaches(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.SetLastException(null, new()));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetLastException(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.IsParallel(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetEngine(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetInputType(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetOutputType(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetLogger(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetAsyncEngine<TestInput, TestOutput>(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetEngine<TestInput, TestOutput>(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.IsAsync(null));
    Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.GetTraceId(null));
    Assert.Throws<ArgumentNullException>(() => new EngineContext().GetOrSet<string>("test", null));

  }
}