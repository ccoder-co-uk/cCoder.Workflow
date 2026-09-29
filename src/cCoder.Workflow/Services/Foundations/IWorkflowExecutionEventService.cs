// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Eventing.Models;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Services.Foundations;

internal interface IWorkflowExecutionEventService
{
    ValueTask RaiseWorkflowRequestEventMessageAsync(EventMessage<WorkflowRequest> message);
}