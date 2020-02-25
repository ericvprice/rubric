using System;
using Microsoft.Extensions.Logging;

namespace RulesEngine.Tests
{
    public class TestLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => new TestScope();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                                Func<TState, Exception, string> formatter) { }
    }
}