// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace cCoder.Workflow.Engine.Brokers.Loggings;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger) : ILoggingBroker
{
    public void LogDebug(string message, params object[] args) =>
        logger.LogDebug(message: message, args: args);

    public void LogInformation(string message, params object[] args) =>
        logger.LogInformation(message: message, args: args);

    public void LogWarning(string message, params object[] args) =>
        logger.LogWarning(message: message, args: args);

    public void LogError(string message, params object[] args) =>
        logger.LogError(message: message, args: args);

    public void LogError(Exception exception, string message, params object[] args) =>
        logger.LogError(exception: exception, message: message, args: args);
}