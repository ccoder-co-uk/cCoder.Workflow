// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Services.Coordinations;

namespace cCoder.Workflow.Exposures;

internal sealed class WorkflowEventHandler(
    IWorkflowEventCoordinationService workflowEventCoordinationService)
    : IWorkflowEventHandler
{
    public Task RaiseEvents(
        object payload,
        string eventName,
        int? appIdOverride = null) =>
        workflowEventCoordinationService.RaiseEvents(
            payload: payload,
            eventName: eventName,
            appIdOverride: appIdOverride);
}