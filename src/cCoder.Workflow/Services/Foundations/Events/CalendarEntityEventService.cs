// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Brokers.Events;
using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;


namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class CalendarEntityEventService(
    ICalendarEntityEventBroker calendarEventBroker)
        : ICalendarEntityEventService
{
    public ValueTask RaiseCalendarAddEventAsync(Calendar entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<Calendar> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = calendarEventBroker.GetCurrentUserId() },
                Data = entity,
            };

            await calendarEventBroker.RaiseCalendarAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarUpdateEventAsync(Calendar entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<Calendar> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = calendarEventBroker.GetCurrentUserId() },
                Data = entity,
            };

            await calendarEventBroker.RaiseCalendarUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarDeleteEventAsync(Calendar entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<Calendar> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = calendarEventBroker.GetCurrentUserId() },
                Data = entity,
            };

            await calendarEventBroker.RaiseCalendarDeleteEventAsync(message: message);
        }, isValueTask: true);
}