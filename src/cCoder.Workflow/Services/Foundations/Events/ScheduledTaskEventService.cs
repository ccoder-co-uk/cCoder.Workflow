// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using cCoder.Workflow.Brokers.Events;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class ScheduledTaskEventService(
    IScheduledTaskEventBroker scheduledTaskEventBroker)
        : IScheduledTaskEventService
{
    public ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [scheduledTask]);

            EventMessage<ScheduledTask> message = CreateScheduledTaskMessage(entity: scheduledTask);

            await scheduledTaskEventBroker.RaiseScheduledTaskAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [scheduledTask]);

            EventMessage<ScheduledTask> message = CreateScheduledTaskMessage(entity: scheduledTask);

            await scheduledTaskEventBroker.RaiseScheduledTaskUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [scheduledTask]);

            EventMessage<ScheduledTask> message = CreateScheduledTaskMessage(entity: scheduledTask);

            await scheduledTaskEventBroker.RaiseScheduledTaskDeleteEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask scheduledTask) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [scheduledTask]);

            EventMessage<ScheduledTask> message = CreateScheduledTaskMessage(entity: scheduledTask);

            await scheduledTaskEventBroker.RaiseScheduledTaskExecuteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<ScheduledTask> CreateScheduledTaskMessage(ScheduledTask entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = scheduledTaskEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}