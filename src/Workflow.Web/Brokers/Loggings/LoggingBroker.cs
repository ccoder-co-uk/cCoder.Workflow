// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;

using System;

using cCoder.CodeAnalysis.Exposures;

namespace Workflow.Web.Brokers.Loggings;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger)
    : ILoggingBroker,
      IUtilityBroker
{
    public void LogInformation(string message) =>
        logger.LogInformation(message: message);

    public void LogWarning(Exception exception, string message) =>
        logger.LogWarning(exception: exception, message: message);

    public void LogWarning(string message) =>
        logger.LogWarning(message: message);

    public void LogError(Exception exception, string message) =>
        logger.LogError(exception: exception, message: message);
}