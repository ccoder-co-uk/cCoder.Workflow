// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Processings;

internal interface IWorkflowEventEventProcessingService
{
    ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent workflowEvent);

    ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent workflowEvent);

    ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent workflowEvent);
}