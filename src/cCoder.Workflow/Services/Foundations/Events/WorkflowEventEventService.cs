// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Eventing.Models;
using cCoder.Workflow.Brokers.Events;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class WorkflowEventEventService(
    IWorkflowEventEventBroker workflowEventEventBroker)
        : IWorkflowEventEventService
{
    public ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowEvent]);

            EventMessage<WorkflowEvent> message = CreateWorkflowEventMessage(entity: workflowEvent);

            await workflowEventEventBroker.RaiseWorkflowEventAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowEvent]);

            EventMessage<WorkflowEvent> message = CreateWorkflowEventMessage(entity: workflowEvent);

            await workflowEventEventBroker.RaiseWorkflowEventUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [workflowEvent]);

            EventMessage<WorkflowEvent> message = CreateWorkflowEventMessage(entity: workflowEvent);

            await workflowEventEventBroker.RaiseWorkflowEventDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<WorkflowEvent> CreateWorkflowEventMessage(WorkflowEvent entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = workflowEventEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}