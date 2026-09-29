// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Eventing.Models;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Brokers.Events;

public interface IWorkflowExecutionEventBroker
{
    ValueTask RaiseWorkflowExecuteEventAsync(
        EventMessage<WorkflowRequest> message);
}