// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal sealed partial class ScheduledTaskEventProcessingService(IScheduledTaskEventService eventService) : IScheduledTaskEventProcessingService
{
    public ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTask]); await ExecuteRaiseScheduledTaskAddEventAsync(entity: scheduledTask); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskAddEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskAddEventAsync(scheduledTask: entity);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTask]); await ExecuteRaiseScheduledTaskUpdateEventAsync(entity: scheduledTask); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskUpdateEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskUpdateEventAsync(scheduledTask: entity);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTask]); await ExecuteRaiseScheduledTaskDeleteEventAsync(entity: scheduledTask); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskDeleteEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskDeleteEventAsync(scheduledTask: entity);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () => { ValidateInputs(inputs: [scheduledTask]); await ExecuteRaiseScheduledTaskExecuteEventAsync(entity: scheduledTask); }, isValueTask: true);

    private ValueTask ExecuteRaiseScheduledTaskExecuteEventAsync(ScheduledTask entity) =>
        eventService.RaiseScheduledTaskExecuteEventAsync(scheduledTask: entity);
}