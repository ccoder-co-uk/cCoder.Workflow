// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal class ScheduledTaskEventProcessingService(IScheduledTaskEventService eventService) : IScheduledTaskEventProcessingService
{
    public ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskAddEventAsync(entity: entity);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskUpdateEventAsync(entity: entity);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskDeleteEventAsync(entity: entity);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskExecuteEventAsync(entity: entity);
}