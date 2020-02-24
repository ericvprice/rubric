using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace RulesEngine.Tests
{

    public class ContextTests
    {

        [Fact]
        public void ReadWrite()
        {
            IEngineContext context = new EngineContext(NullLogger.Instance);
            Assert.Equal(context.Logger, NullLogger.Instance);
            Assert.False(context.ContainsKey("test"));
            context["test"] = true;
            Assert.True(context.ContainsKey("test"));
            Assert.True(context.Get<bool>("test"));
            Assert.True((bool)context["test"]);
            context["test"] = false;
            Assert.False(context.Get<bool>("test"));
            Assert.False((bool)context["test"]);
            context.Remove("test");
            Assert.False(context.ContainsKey("test"));
        }

        [Fact]
        public void Clone()
        {
            IEngineContext context = new EngineContext(NullLogger.Instance) {["foo"] = "bar", ["baz"] = "qux"};
            var cloned = context.Clone();
            Assert.Equal(context.Logger, cloned.Logger);
            Assert.Equal(context.Get<string>("foo"), cloned.Get<string>("foo"));
            Assert.Equal(context.Get<string>("baz"), cloned.Get<string>("baz"));
        }

        [Fact]
        public void Constructor()
        {
            IEngineContext context = new EngineContext();
            Assert.NotNull(context.Logger);
            var logger = new TestLogger();
            context = new EngineContext(logger);
            Assert.Equal(logger, context.Logger);

        }
    }
}