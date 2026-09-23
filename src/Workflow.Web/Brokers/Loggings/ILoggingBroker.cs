// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Workflow.Web.Brokers.Loggings;

public interface ILoggingBroker
{
    void LogError(Exception exception, string message);
}