// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Workflow.Exposures;

public interface IWorkflowEventHandler
{
    Task RaiseEvents(
        object payload,
        string eventName,
        int? appIdOverride = null);
}