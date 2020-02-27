using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using RulesEngine;

namespace RulesEngine.Tests
{
    public class ContextTests
    {
        [Fact]
        public void Clone()
        {
            IEngineContext context = new EngineContext { ["foo"] = "bar", ["baz"] = "qux" };
            var cloned = context.Clone();
            Assert.Equal(context.Get<string>("foo"), cloned.Get<string>("foo"));
            Assert.Equal(context.Get<string>("baz"), cloned.Get<string>("baz"));
        }
        
        [Fact]
        public void ReadWrite()
        {
            IEngineContext context = new EngineContext();
            Assert.False(context.ContainsKey("test"));
            context["test"] = true;
            Assert.True(context.ContainsKey("test"));
            Assert.True(context.Get<bool>("test"));
            Assert.True((bool) context["test"]);
            context["test"] = false;
            Assert.False(context.Get<bool>("test"));
            Assert.False((bool) context["test"]);
            context.Remove("test");
            Assert.False(context.ContainsKey("test"));
        }

        [Fact]
        public void ContextExtensions()
        {
            IEngineContext ctx = new EngineContext();
            var logger = new TestLogger();
            var engine = new RulesEngine<TestInput, TestOutput>(null, null, null, logger);
            engine.SetupContext(ctx);
            Assert.Equal(engine, ctx.GetEngine());
            Assert.Equal(engine, ctx.GetEngine<TestInput, TestOutput>());
            Assert.Equal(logger, ctx.GetLogger());
            Assert.False(ctx.IsAsync());
            Assert.False(ctx.IsParallel());
            Assert.Equal(typeof(TestInput), ctx.GetInputType());
            Assert.Equal(typeof(TestOutput), ctx.GetOutputType());
        }

        [Fact]
        public void AsycContextExtensions()
        {
            IEngineContext ctx = new EngineContext();
            var logger = new TestLogger();
            var engine = new AsyncRulesEngine<TestInput, TestOutput>(null, null, null, logger);
            engine.SetupContext(ctx);
            Assert.Equal(engine, ctx.GetEngine());
            Assert.Equal(engine, ctx.GetAsyncEngine<TestInput, TestOutput>());
            Assert.Equal(logger, ctx.GetLogger());
            Assert.True(ctx.IsAsync());
            Assert.False(ctx.IsParallel());
            engine.IsParallel = true;
            Assert.True(ctx.IsParallel());
            Assert.Equal(typeof(TestInput), ctx.GetInputType());
            Assert.Equal(typeof(TestOutput), ctx.GetOutputType());
        }
    }
}