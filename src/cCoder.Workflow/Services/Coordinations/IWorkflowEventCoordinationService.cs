// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Services.Coordinations;

internal interface IWorkflowEventCoordinationService
{
    Task RaiseEvents(object payload, string eventName, int? appIdOverride = null);
}