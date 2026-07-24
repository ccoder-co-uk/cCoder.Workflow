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
    public ValueTask RaiseScheduledTaskAddEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<ScheduledTask> message = CreateMessage(entity: entity);

            await scheduledTaskEventBroker.RaiseScheduledTaskAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseScheduledTaskUpdateEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<ScheduledTask> message = CreateMessage(entity: entity);

            await scheduledTaskEventBroker.RaiseScheduledTaskUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseScheduledTaskDeleteEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<ScheduledTask> message = CreateMessage(entity: entity);

            await scheduledTaskEventBroker.RaiseScheduledTaskDeleteEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseScheduledTaskExecuteEventAsync(ScheduledTask entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<ScheduledTask> message = CreateMessage(entity: entity);

            await scheduledTaskEventBroker.RaiseScheduledTaskExecuteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<ScheduledTask> CreateMessage(ScheduledTask entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = scheduledTaskEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}