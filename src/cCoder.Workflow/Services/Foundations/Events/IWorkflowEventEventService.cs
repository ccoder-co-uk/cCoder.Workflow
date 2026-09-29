// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

internal interface IWorkflowEventEventService
{
    ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent workflowEvent);

    ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent workflowEvent);

    ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent workflowEvent);
}