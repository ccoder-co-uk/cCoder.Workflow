// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Brokers.Events;

public interface IWorkflowExecutionEventBroker
{
    ValueTask RaiseWorkflowExecuteEventAsync(
        EventMessage<WorkflowRequest> message);
}