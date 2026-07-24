// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Orchestrations;

public interface IEventHandlingOrchestrationService
{
    Task RaiseEvents(object payload, string eventName, int? appIdOverride = null);
}