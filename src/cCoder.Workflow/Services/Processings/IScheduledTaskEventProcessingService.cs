// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Processings;

internal interface IScheduledTaskEventProcessingService
{
    ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask scheduledTask);

    ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask scheduledTask);
}