using Microsoft.Extensions.Logging;

namespace Rubric;

internal static class LoggerExtensions
{
  internal static IDisposable BeginScope(this ILogger logger, string key, object obj)
      => logger.BeginScope(new Dictionary<string, object> { { key, obj } });
}
