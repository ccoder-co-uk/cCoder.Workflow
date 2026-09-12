// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class WorkflowEventEventProcessingService(IWorkflowEventEventService eventService)
    : IWorkflowEventEventProcessingService
{
    public ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEvent]); await ExecuteRaiseWorkflowEventAddEventAsync(entity: workflowEvent); }, isValueTask: true);

    private ValueTask ExecuteRaiseWorkflowEventAddEventAsync(WorkflowEvent entity) =>
        eventService.RaiseWorkflowEventAddEventAsync(workflowEvent: entity);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEvent]); await ExecuteRaiseWorkflowEventUpdateEventAsync(entity: workflowEvent); }, isValueTask: true);

    private ValueTask ExecuteRaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity) =>
        eventService.RaiseWorkflowEventUpdateEventAsync(workflowEvent: entity);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent workflowEvent) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [workflowEvent]); await ExecuteRaiseWorkflowEventDeleteEventAsync(entity: workflowEvent); }, isValueTask: true);

    private ValueTask ExecuteRaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity) =>
        eventService.RaiseWorkflowEventDeleteEventAsync(workflowEvent: entity);
}