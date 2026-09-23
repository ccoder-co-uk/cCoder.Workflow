// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace Workflow.HostedServices.Brokers.Loggings;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger)
    : ILoggingBroker,
      IUtilityBroker
{
    public void LogError(Exception exception, string message) =>
        logger.LogError(exception: exception, message: message);
}