using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using NSubstitute;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public static class NSubsituteExtensions
    {
        public static void WasCalledOnce(this ILogger logger, LogLevel level, string msg)
        {
            Assert.Single(logger.ReceivedCalls(), call =>
            {
                var args = call.GetArguments();
                var logLevel = (LogLevel)args[0];
                var message = (args[2] as FormattedLogValues)?.ToString();
                return logLevel == level && message == msg;
            });
        }
    }
}