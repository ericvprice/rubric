using Rubric.Engines.Implementation;

namespace Rubric.Tests.Core;

public class ContextTests
{
  [Fact]
  public void Clone()
  {
    IEngineContext context = new EngineContext { ["foo"] = "bar", ["baz"] = "qux" };
    var cloned = context.Clone();
    Assert.Equal(context.GetAs<string>("foo"), cloned.GetAs<string>("foo"));
    Assert.Equal(context.GetAs<string>("baz"), cloned.GetAs<string>("baz"));
  }

  [Fact]
  public void ReadWrite()
  {
    IEngineContext context = new EngineContext();
    Assert.False(context.ContainsKey("test"));
    context["test"] = true;
    Assert.True(context.ContainsKey("test"));
    Assert.True(context.GetAs<bool>("test"));
    Assert.True((bool)context["test"]);
    context["test"] = false;
    Assert.False(context.GetAs<bool>("test"));
    Assert.False((bool)context["test"]);
    context.Remove("test");
    Assert.False(context.ContainsKey("test"));
  }

  [Fact]
  public void ContextExtensions()
  {
    IEngineContext ctx = new EngineContext();
    var logger = new TestLogger();
    var engine = new RuleEngine<TestInput, TestOutput>(null, null, null, null, logger);
    engine.SetupContext(ctx);
    Assert.Equal(engine, ctx.GetEngine());
    Assert.Equal(engine, ctx.GetEngine<TestInput, TestOutput>());
    Assert.Equal(logger, ctx.GetLogger());
    Assert.False(ctx.IsAsync());
    Assert.False(ctx.IsParallel());
    Assert.Equal(typeof(TestInput), ctx.GetInputType());
    Assert.Equal(typeof(TestOutput), ctx.GetOutputType());
  }

  //[Fact]
  //public void Exceptions()
  //{
  //  var logger = new TestLogger();
  //  var engine = new RuleEngine<TestInput, TestOutput>(null, null, null, null, logger);
  //  Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.SetExecutionInfo(new EngineContext(), null, null));
  //  Assert.Throws<ArgumentNullException>(() => EngineContextExtensions.SetExecutionInfo(null, engine, null));
  //}
}

