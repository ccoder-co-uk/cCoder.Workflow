// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace Workflow.Web.Brokers.Loggings;

public interface ILoggingBroker
{
    void LogInformation(string message);
    void LogWarning(Exception exception, string message);
    void LogWarning(string message);
    void LogError(Exception exception, string message);
}