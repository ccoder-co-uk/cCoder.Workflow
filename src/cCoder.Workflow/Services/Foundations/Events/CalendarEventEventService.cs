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
    public ValueTask RaiseCalendarEventAddEventAsync(CalendarEvent entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<CalendarEvent> message = CreateMessage(entity: entity);

            await calendarEventEventBroker.RaiseCalendarEventAddEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarEventUpdateEventAsync(CalendarEvent entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<CalendarEvent> message = CreateMessage(entity: entity);

            await calendarEventEventBroker.RaiseCalendarEventUpdateEventAsync(message: message);
        }, isValueTask: true);

    public ValueTask RaiseCalendarEventDeleteEventAsync(CalendarEvent entity) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [entity]);

            EventMessage<CalendarEvent> message = CreateMessage(entity: entity);

            await calendarEventEventBroker.RaiseCalendarEventDeleteEventAsync(message: message);
        }, isValueTask: true);

    private EventMessage<CalendarEvent> CreateMessage(CalendarEvent entity) =>
        new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = calendarEventEventBroker.GetCurrentUserId()
            },
            Data = entity,
        };
}