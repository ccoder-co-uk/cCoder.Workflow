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
    public ValueTask RaiseCalendarAddEventAsync(Calendar calendar) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [calendar]);

            EventMessage<Calendar> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = calendarEventBroker.GetCurrentUserId() },
                Data = calendar,
            };

            await calendarEventBroker.RaiseCalendarAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarUpdateEventAsync(Calendar calendar) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [calendar]);

            EventMessage<Calendar> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = calendarEventBroker.GetCurrentUserId() },
                Data = calendar,
            };

            await calendarEventBroker.RaiseCalendarUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarDeleteEventAsync(Calendar calendar) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [calendar]);

            EventMessage<Calendar> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = calendarEventBroker.GetCurrentUserId() },
                Data = calendar,
            };

            await calendarEventBroker.RaiseCalendarDeleteEventAsync(message: message);
        }, isValueTask: true);
}