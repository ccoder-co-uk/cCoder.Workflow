// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Foundations.Events;

internal interface IScheduledTaskEventService
{
    ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask scheduledTask);
}