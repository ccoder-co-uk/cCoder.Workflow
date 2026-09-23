// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowExecutionEventService
{
    ValueTask RaiseWorkflowRequestEventMessageAsync(EventMessage<WorkflowRequest> message);
}