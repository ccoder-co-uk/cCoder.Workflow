using cCoder.Data.Models.Planning;
using cCoder.Workflow.Services.Foundations.Events;


namespace cCoder.Workflow.Services.Processings;

internal class ScheduledTaskEventProcessingService(IScheduledTaskEventService eventService) : IScheduledTaskEventProcessingService
{
    public ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskAddEventAsync(entity);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskUpdateEventAsync(entity);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskDeleteEventAsync(entity);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask entity) => eventService.RaiseScheduledTaskExecuteEventAsync(entity);
}









