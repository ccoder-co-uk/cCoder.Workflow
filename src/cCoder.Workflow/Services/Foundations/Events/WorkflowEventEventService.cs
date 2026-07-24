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
    public ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<WorkflowEvent> message = CreateMessage(entity: entity);

            await workflowEventEventBroker.RaiseWorkflowEventAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<WorkflowEvent> message = CreateMessage(entity: entity);

            await workflowEventEventBroker.RaiseWorkflowEventUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<WorkflowEvent> message = CreateMessage(entity: entity);

            await workflowEventEventBroker.RaiseWorkflowEventDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<WorkflowEvent> CreateMessage(WorkflowEvent entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = workflowEventEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}