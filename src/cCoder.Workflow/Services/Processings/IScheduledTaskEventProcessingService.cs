// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Services.Processings;

internal interface IScheduledTaskEventProcessingService
{
    ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask scheduledTask);
}