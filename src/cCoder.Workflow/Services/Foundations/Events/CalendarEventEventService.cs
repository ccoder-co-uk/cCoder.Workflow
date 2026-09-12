// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Planning;
using cCoder.Eventing.Models;
using cCoder.Workflow.Brokers.Events;

namespace cCoder.Workflow.Services.Foundations.Events;

internal sealed partial class CalendarEventEventService(
    ICalendarEventEventBroker calendarEventEventBroker)
        : ICalendarEventEventService
{
    public ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [calendarEvent]);

            EventMessage<CalendarEvent> message = CreateCalendarEventMessage(entity: calendarEvent);

            await calendarEventEventBroker.RaiseCalendarEventAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [calendarEvent]);

            EventMessage<CalendarEvent> message = CreateCalendarEventMessage(entity: calendarEvent);

            await calendarEventEventBroker.RaiseCalendarEventUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent calendarEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [calendarEvent]);

            EventMessage<CalendarEvent> message = CreateCalendarEventMessage(entity: calendarEvent);

            await calendarEventEventBroker.RaiseCalendarEventDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<CalendarEvent> CreateCalendarEventMessage(CalendarEvent entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = calendarEventEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}