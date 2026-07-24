// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal sealed partial class ScheduledTaskEventProcessingService(IScheduledTaskEventService eventService) : IScheduledTaskEventProcessingService
{
    public ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseScheduledTaskAddEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskAddEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskAddEventAsync(entity: entity);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseScheduledTaskUpdateEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskUpdateEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskUpdateEventAsync(entity: entity);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseScheduledTaskDeleteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskDeleteEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskDeleteEventAsync(entity: entity);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [entity]); await ExecuteRaiseScheduledTaskExecuteEventAsync(entity: entity); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskExecuteEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskExecuteEventAsync(entity: entity);
}