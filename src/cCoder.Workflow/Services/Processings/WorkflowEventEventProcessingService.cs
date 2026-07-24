// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Services.Foundations.Events;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class WorkflowEventEventProcessingService(IWorkflowEventEventService eventService)
    : IWorkflowEventEventProcessingService
{
    public ValueTask RaiseWorkflowEventAddEventAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseWorkflowEventAddEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseWorkflowEventAddEventAsync(WorkflowEvent entity) =>
        eventService.RaiseWorkflowEventAddEventAsync(entity: entity);

    public ValueTask RaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseWorkflowEventUpdateEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseWorkflowEventUpdateEventAsync(WorkflowEvent entity) =>
        eventService.RaiseWorkflowEventUpdateEventAsync(entity: entity);

    public ValueTask RaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseWorkflowEventDeleteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseWorkflowEventDeleteEventAsync(WorkflowEvent entity) =>
        eventService.RaiseWorkflowEventDeleteEventAsync(entity: entity);
}