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
    public void LogError(Exception exception, string message) =>
        logger.LogError(exception: exception, message: message);
}