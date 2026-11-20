// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace Workflow.HostedServices.Brokers.Loggings;

public interface ILoggingBroker
{
    void LogWarning(Exception exception, string message);
    void LogError(Exception exception, string message);
}