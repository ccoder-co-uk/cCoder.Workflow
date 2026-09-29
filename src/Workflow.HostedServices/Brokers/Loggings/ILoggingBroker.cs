// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace Workflow.HostedServices.Brokers.Loggings;

public interface ILoggingBroker
{
    void LogError(Exception exception, string message);
}