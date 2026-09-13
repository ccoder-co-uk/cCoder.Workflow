// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Exposures;

public interface IWorkflowEventHandler
{
    Task RaiseEvents(
        object payload,
        string eventName,
        int? appIdOverride = null);
}