// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Services.Foundations.Events;

internal class ScheduledTaskEventService(
    IScheduledTaskEventBroker scheduledTaskEventBroker,
    ICoreAuthInfo authInfo
) : IScheduledTaskEventService
{
    public async ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask entity)
    {
        EventMessage<ScheduledTask> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await scheduledTaskEventBroker.RaiseScheduledTaskAddEventAsync(message);
    }

    public async ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask entity)
    {
        EventMessage<ScheduledTask> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await scheduledTaskEventBroker.RaiseScheduledTaskUpdateEventAsync(message);
    }

    public async ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask entity)
    {
        EventMessage<ScheduledTask> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await scheduledTaskEventBroker.RaiseScheduledTaskDeleteEventAsync(message);
    }

    public async ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask entity)
    {
        EventMessage<ScheduledTask> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = entity,
        };

        await scheduledTaskEventBroker.RaiseScheduledTaskExecuteEventAsync(message);
    }
}